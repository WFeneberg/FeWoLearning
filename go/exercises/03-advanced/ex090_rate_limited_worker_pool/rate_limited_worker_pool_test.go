package ratelimitedworkerpool

import (
	"context"
	"testing"
	"time"
)

// recvResult takes the next Result off results, failing the test rather than
// hanging forever if the pool stops making progress.
func recvResult(t *testing.T, results <-chan Result) Result {
	t.Helper()
	select {
	case r, ok := <-results:
		if !ok {
			t.Fatal("results channel closed before all jobs completed")
		}
		return r
	case <-time.After(5 * time.Second):
		t.Fatal("timed out waiting for the next result: the pool made no progress")
		return Result{}
	}
}

// runCapped drives a Pool of `workers` goroutines over `numJobs` jobs through a
// RateLimiter of burst `capacity`, feeding ticks from the test goroutine
// without ever sleeping or reading the wall clock.
//
// The tick schedule is what makes this deterministic. The limiter starts with
// `capacity` permits and drops any tick that arrives while the bucket is full,
// so the test must never tick into a full bucket — otherwise the permit is
// silently lost, the pool starves, and no number of further ticks recovers a
// one-permit-per-tick accounting. Therefore:
//
//  1. First drain the initial burst: read min(capacity, numJobs) results. Each
//     result proves a worker consumed one permit, so once they are all in, the
//     bucket is empty.
//  2. Then send exactly one tick per remaining job and read exactly one result
//     for it. With an empty bucket every tick grants exactly one permit, so
//     completed jobs can never outrun capacity + ticksSent.
func runCapped(t *testing.T, capacity, workers, numJobs int) (completed, ticksSent int) {
	t.Helper()

	ticks := make(chan time.Time)
	limiter := NewRateLimiter(capacity, ticks)
	defer limiter.Close()

	pool := NewPool(workers, limiter)

	jobs := make([]Job, numJobs)
	for i := range jobs {
		id := i
		jobs[i] = Job{ID: id, Fn: func() int { return id * id }}
	}

	results := pool.Run(context.Background(), jobs)

	seen := make(map[int]bool, numJobs)
	record := func(r Result) {
		if seen[r.JobID] {
			t.Fatalf("job %d completed more than once", r.JobID)
		}
		seen[r.JobID] = true
		if want := r.JobID * r.JobID; r.Value != want {
			t.Fatalf("job %d value = %d, want %d", r.JobID, r.Value, want)
		}
		completed++
		if completed > capacity+ticksSent {
			t.Fatalf(
				"rate cap exceeded: %d jobs completed after only %d ticks (burst capacity %d)",
				completed, ticksSent, capacity,
			)
		}
	}

	burst := capacity
	if numJobs < burst {
		burst = numJobs
	}
	for i := 0; i < burst; i++ {
		record(recvResult(t, results))
	}

	for completed < numJobs {
		ticks <- time.Time{}
		ticksSent++
		record(recvResult(t, results))
	}

	// Every job is accounted for, so the pool must now shut the channel.
	select {
	case _, ok := <-results:
		if ok {
			t.Fatal("received an extra result after every job had completed")
		}
	case <-time.After(5 * time.Second):
		t.Fatal("results channel was not closed after all jobs completed")
	}

	return completed, ticksSent
}

func TestRateLimiterNeverExceedsCap(t *testing.T) {
	cases := []struct {
		name     string
		capacity int
		workers  int
		numJobs  int
	}{
		{"single_worker_tight_cap", 1, 1, 6},
		{"contended_workers_small_burst", 2, 5, 10},
		{"burst_covers_all_jobs", 4, 3, 4},
		{"many_workers_wide_backlog", 3, 8, 15},
	}

	for _, tc := range cases {
		t.Run(tc.name, func(t *testing.T) {
			completed, ticksSent := runCapped(t, tc.capacity, tc.workers, tc.numJobs)

			if completed != tc.numJobs {
				t.Fatalf("completed = %d, want all %d jobs to finish", completed, tc.numJobs)
			}

			// One tick per job beyond the initial burst — no more, no fewer.
			wantTicks := tc.numJobs - tc.capacity
			if wantTicks < 0 {
				wantTicks = 0
			}
			if ticksSent != wantTicks {
				t.Fatalf("ticksSent = %d, want exactly %d", ticksSent, wantTicks)
			}
		})
	}
}

func TestRateLimiterAcquireRespectsContextCancellation(t *testing.T) {
	ticks := make(chan time.Time)
	limiter := NewRateLimiter(1, ticks)
	defer limiter.Close()

	// Drain the sole starting permit so a further Acquire must block.
	ctx := context.Background()
	if err := limiter.Acquire(ctx); err != nil {
		t.Fatalf("first Acquire: unexpected error %v", err)
	}

	cancelCtx, cancel := context.WithCancel(context.Background())
	cancel()

	if err := limiter.Acquire(cancelCtx); err == nil {
		t.Fatal("Acquire on an already-canceled context: want error, got nil")
	}
}

func TestNewRateLimiterPanicsOnNonPositiveCapacity(t *testing.T) {
	defer func() {
		if recover() == nil {
			t.Fatal("NewRateLimiter(0, ...): want panic, got none")
		}
	}()
	NewRateLimiter(0, make(chan time.Time))
}

func TestNewPoolPanicsOnInvalidArgs(t *testing.T) {
	limiter := NewRateLimiter(1, make(chan time.Time))
	defer limiter.Close()

	t.Run("zero_workers", func(t *testing.T) {
		defer func() {
			if recover() == nil {
				t.Fatal("NewPool(0, limiter): want panic, got none")
			}
		}()
		NewPool(0, limiter)
	})

	t.Run("nil_limiter", func(t *testing.T) {
		defer func() {
			if recover() == nil {
				t.Fatal("NewPool(1, nil): want panic, got none")
			}
		}()
		NewPool(1, nil)
	})
}

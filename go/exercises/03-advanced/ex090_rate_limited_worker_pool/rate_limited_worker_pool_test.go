package ratelimitedworkerpool

import (
	"context"
	"runtime"
	"testing"
	"time"
)

// runCapped drives a Pool of `workers` goroutines over `numJobs` jobs through
// a RateLimiter of the given burst `capacity`, feeding ticks to the limiter
// one at a time from the test goroutine (never sleeping or reading the wall
// clock). It continuously verifies the safety invariant that must hold no
// matter how goroutines are scheduled: the number of jobs completed so far
// can never exceed capacity + the number of ticks sent so far, since the
// limiter can never have issued more permits than that. It returns the final
// count of completed jobs and ticks sent.
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

	for {
		select {
		case r, ok := <-results:
			if !ok {
				return completed, ticksSent
			}
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
		default:
			if ticksSent < numJobs {
				ticks <- time.Time{}
				ticksSent++
			} else {
				runtime.Gosched()
			}
		}
	}
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

			needed := tc.numJobs - tc.capacity
			if needed > 0 && ticksSent < needed {
				t.Fatalf(
					"expected at least %d ticks to drain the backlog beyond the initial burst, only sent %d",
					needed, ticksSent,
				)
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

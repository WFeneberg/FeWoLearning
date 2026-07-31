// Package ratelimitedworkerpool — Exercise 090 (reference solution).
package ratelimitedworkerpool

import (
	"context"
	"sync"
	"time"
)

// Job is a unit of work submitted to the pool.
type Job struct {
	ID int
	Fn func() int
}

// Result is the output produced by running a Job.
type Result struct {
	JobID int
	Value int
}

// RateLimiter is a token-bucket limiter with a fixed burst capacity. It
// starts full (capacity permits available immediately) and gains at most one
// additional permit each time a value arrives on ticks, up to capacity.
// Permits beyond capacity are dropped, so the limiter never issues more than
// capacity + (number of ticks so far) permits in total.
type RateLimiter struct {
	tokens chan struct{}
	stop   chan struct{}
	once   sync.Once
}

// NewRateLimiter creates a limiter with the given burst capacity, refilled
// by one token each time a value arrives on ticks. It panics if capacity is
// not positive.
func NewRateLimiter(capacity int, ticks <-chan time.Time) *RateLimiter {
	if capacity <= 0 {
		panic("ratelimitedworkerpool: capacity must be positive")
	}
	rl := &RateLimiter{
		tokens: make(chan struct{}, capacity),
		stop:   make(chan struct{}),
	}
	// Start full: a burst of `capacity` permits is available immediately.
	for i := 0; i < capacity; i++ {
		rl.tokens <- struct{}{}
	}
	go rl.refill(ticks)
	return rl
}

func (rl *RateLimiter) refill(ticks <-chan time.Time) {
	for {
		select {
		case <-rl.stop:
			return
		case _, ok := <-ticks:
			if !ok {
				return
			}
			select {
			case rl.tokens <- struct{}{}:
			default:
				// Bucket already full; this tick's permit is dropped.
			}
		}
	}
}

// Acquire blocks until a token is available or ctx is done, whichever comes
// first, returning ctx.Err() in the latter case.
func (rl *RateLimiter) Acquire(ctx context.Context) error {
	select {
	case <-rl.tokens:
		return nil
	case <-ctx.Done():
		return ctx.Err()
	}
}

// Close stops the limiter's internal refill goroutine. Safe to call once;
// additional calls must not panic.
func (rl *RateLimiter) Close() {
	rl.once.Do(func() { close(rl.stop) })
}

// Pool runs Jobs across a fixed number of worker goroutines, throttling
// execution through a shared RateLimiter.
type Pool struct {
	workers int
	limiter *RateLimiter
}

// NewPool creates a pool with the given worker count and shared limiter. It
// panics if workers is not positive or limiter is nil.
func NewPool(workers int, limiter *RateLimiter) *Pool {
	if workers <= 0 {
		panic("ratelimitedworkerpool: workers must be positive")
	}
	if limiter == nil {
		panic("ratelimitedworkerpool: limiter must not be nil")
	}
	return &Pool{workers: workers, limiter: limiter}
}

// Run dispatches all jobs across the pool's workers and returns a channel of
// results, closed once every job has completed (or ctx is canceled). Each
// worker must acquire a permit from the pool's RateLimiter before running a
// job's Fn, so completed throughput never exceeds the limiter's cap. Results
// may arrive in any order.
func (p *Pool) Run(ctx context.Context, jobs []Job) <-chan Result {
	in := make(chan Job)
	out := make(chan Result, len(jobs))

	var wg sync.WaitGroup
	wg.Add(p.workers)
	for i := 0; i < p.workers; i++ {
		go func() {
			defer wg.Done()
			for job := range in {
				if err := p.limiter.Acquire(ctx); err != nil {
					return
				}
				out <- Result{JobID: job.ID, Value: job.Fn()}
			}
		}()
	}

	go func() {
		defer close(in)
		for _, j := range jobs {
			select {
			case in <- j:
			case <-ctx.Done():
				return
			}
		}
	}()

	go func() {
		wg.Wait()
		close(out)
	}()

	return out
}

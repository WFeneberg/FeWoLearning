// Package ratelimitedworkerpool — Exercise 090 (advanced).
// Goal:   A worker pool that processes jobs through a shared token-bucket
//         rate limiter capping throughput to a configured rate. The limiter
//         is refilled entirely by an externally supplied tick source (in
//         production a time.Ticker's channel, in tests a manually driven
//         channel), so the limiting logic never itself reads the wall
//         clock and stays deterministically testable.
// Drills: goroutines, channels, select, context cancellation, worker pools,
//         token-bucket rate limiting.
package ratelimitedworkerpool

import (
	"context"
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
	// TODO: add fields (token buffer, stop channel, ...)
}

// NewRateLimiter creates a limiter with the given burst capacity, refilled
// by one token each time a value arrives on ticks. It panics if capacity is
// not positive.
func NewRateLimiter(capacity int, ticks <-chan time.Time) *RateLimiter {
	panic("TODO: implement NewRateLimiter")
}

// Acquire blocks until a token is available or ctx is done, whichever comes
// first, returning ctx.Err() in the latter case.
func (rl *RateLimiter) Acquire(ctx context.Context) error {
	panic("TODO: implement Acquire")
}

// Close stops the limiter's internal refill goroutine. Safe to call once;
// additional calls must not panic.
func (rl *RateLimiter) Close() {
	panic("TODO: implement Close")
}

// Pool runs Jobs across a fixed number of worker goroutines, throttling
// execution through a shared RateLimiter.
type Pool struct {
	// TODO: add fields (worker count, limiter)
}

// NewPool creates a pool with the given worker count and shared limiter. It
// panics if workers is not positive or limiter is nil.
func NewPool(workers int, limiter *RateLimiter) *Pool {
	panic("TODO: implement NewPool")
}

// Run dispatches all jobs across the pool's workers and returns a channel of
// results, closed once every job has completed (or ctx is canceled). Each
// worker must acquire a permit from the pool's RateLimiter before running a
// job's Fn, so completed throughput never exceeds the limiter's cap. Results
// may arrive in any order.
func (p *Pool) Run(ctx context.Context, jobs []Job) <-chan Result {
	panic("TODO: implement Run")
}

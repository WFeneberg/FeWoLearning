// Package ratelimitertokenbucket — Exercise 075 (advanced).
// Goal:   A TokenBucket rate limiter. Allow() reports whether a request may
//         proceed right now, consuming one token if so. Tokens refill
//         continuously at a fixed rate up to the bucket's capacity.
// Drills: rate limiting algorithms, injectable clocks for deterministic time
//         based tests, floating point accounting.
package ratelimitertokenbucket

import "time"

// TokenBucket is a fixed-capacity, continuously-refilling token bucket
// rate limiter. It is safe only for single-goroutine use in this exercise.
type TokenBucket struct {
	// TODO: add fields (capacity, refillRate, current tokens,
	// last-refill timestamp, and an injectable "now" function)
}

// New creates a TokenBucket with the given capacity (max burst size) and
// refillRate tokens per second. The bucket starts full. It panics if
// capacity <= 0 or refillRate <= 0.
func New(capacity int, refillRate float64) *TokenBucket {
	panic("TODO: implement New")
}

// NewWithClock is like New but lets tests inject a deterministic clock
// instead of time.Now, so behavior can be verified without sleeping.
func NewWithClock(capacity int, refillRate float64, now func() time.Time) *TokenBucket {
	panic("TODO: implement NewWithClock")
}

// Allow reports whether a request is permitted right now. If permitted, it
// atomically consumes one token from the bucket. Tokens are refilled based
// on elapsed time since the last call, up to the bucket's capacity.
func (b *TokenBucket) Allow() bool {
	panic("TODO: implement Allow")
}

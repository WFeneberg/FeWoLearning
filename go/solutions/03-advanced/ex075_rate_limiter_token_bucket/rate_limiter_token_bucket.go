// Package ratelimitertokenbucket — Exercise 075 (reference solution).
package ratelimitertokenbucket

import "time"

// TokenBucket is a fixed-capacity, continuously-refilling token bucket
// rate limiter. It is safe only for single-goroutine use in this exercise.
type TokenBucket struct {
	capacity   float64
	refillRate float64 // tokens per second
	tokens     float64
	last       time.Time
	now        func() time.Time
}

// New creates a TokenBucket with the given capacity (max burst size) and
// refillRate tokens per second. The bucket starts full. It panics if
// capacity <= 0 or refillRate <= 0.
func New(capacity int, refillRate float64) *TokenBucket {
	return NewWithClock(capacity, refillRate, time.Now)
}

// NewWithClock is like New but lets tests inject a deterministic clock
// instead of time.Now, so behavior can be verified without sleeping.
func NewWithClock(capacity int, refillRate float64, now func() time.Time) *TokenBucket {
	if capacity <= 0 {
		panic("capacity must be positive")
	}
	if refillRate <= 0 {
		panic("refillRate must be positive")
	}
	if now == nil {
		panic("now must not be nil")
	}
	return &TokenBucket{
		capacity:   float64(capacity),
		refillRate: refillRate,
		tokens:     float64(capacity),
		last:       now(),
		now:        now,
	}
}

// Allow reports whether a request is permitted right now. If permitted, it
// atomically consumes one token from the bucket. Tokens are refilled based
// on elapsed time since the last call, up to the bucket's capacity.
func (b *TokenBucket) Allow() bool {
	current := b.now()
	elapsed := current.Sub(b.last).Seconds()
	if elapsed > 0 {
		b.tokens += elapsed * b.refillRate
		if b.tokens > b.capacity {
			b.tokens = b.capacity
		}
		b.last = current
	}

	if b.tokens >= 1 {
		b.tokens--
		return true
	}
	return false
}

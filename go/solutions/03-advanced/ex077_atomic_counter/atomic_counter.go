// Package atomiccounter — Exercise 077 (reference solution).
package atomiccounter

import "sync/atomic"

// AtomicCounter is a goroutine-safe counter backed by atomic.Int64.
type AtomicCounter struct {
	value atomic.Int64
}

// NewAtomicCounter returns a counter initialized to 0.
func NewAtomicCounter() *AtomicCounter {
	return &AtomicCounter{}
}

// Increment atomically adds 1 to the counter and returns the new value.
func (c *AtomicCounter) Increment() int64 {
	return c.value.Add(1)
}

// Add atomically adds delta (which may be negative) and returns the new value.
func (c *AtomicCounter) Add(delta int64) int64 {
	return c.value.Add(delta)
}

// Load atomically returns the current value.
func (c *AtomicCounter) Load() int64 {
	return c.value.Load()
}

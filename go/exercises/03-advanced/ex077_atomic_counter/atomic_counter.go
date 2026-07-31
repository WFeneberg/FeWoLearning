// Package atomiccounter — Exercise 077 (advanced).
// Goal:   A goroutine-safe counter built on atomic.Int64 — no mutex required.
// Drills: sync/atomic, lock-free data access, memory model basics.
package atomiccounter

import "sync/atomic"

// AtomicCounter is a goroutine-safe counter backed by atomic.Int64.
type AtomicCounter struct {
	// TODO: add a field of type atomic.Int64
}

// NewAtomicCounter returns a counter initialized to 0.
func NewAtomicCounter() *AtomicCounter {
	panic("TODO: implement NewAtomicCounter")
}

// Increment atomically adds 1 to the counter and returns the new value.
func (c *AtomicCounter) Increment() int64 {
	panic("TODO: implement Increment")
}

// Add atomically adds delta (which may be negative) and returns the new value.
func (c *AtomicCounter) Add(delta int64) int64 {
	panic("TODO: implement Add")
}

// Load atomically returns the current value.
func (c *AtomicCounter) Load() int64 {
	panic("TODO: implement Load")
}

var _ = atomic.Int64{} // keep import until implemented

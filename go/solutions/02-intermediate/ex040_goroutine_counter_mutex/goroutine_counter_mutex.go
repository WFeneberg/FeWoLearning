// Package goroutinecountermutex — Exercise 040 (reference solution).
package goroutinecountermutex

import "sync"

// SafeCounter is a counter safe for concurrent use by multiple goroutines.
type SafeCounter struct {
	mu    sync.Mutex
	count int
}

// Increment adds 1 to the counter in a goroutine-safe way.
func (c *SafeCounter) Increment() {
	c.mu.Lock()
	defer c.mu.Unlock()
	c.count++
}

// Value returns the current counter value in a goroutine-safe way.
func (c *SafeCounter) Value() int {
	c.mu.Lock()
	defer c.mu.Unlock()
	return c.count
}

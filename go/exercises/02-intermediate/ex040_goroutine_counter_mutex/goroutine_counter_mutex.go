// Package goroutinecountermutex — Exercise 040 (intermediate).
// Goal:   Implement a SafeCounter guarded by sync.Mutex so that many
//         goroutines can call Increment concurrently without a data race.
// Drills: sync.Mutex, concurrency safety, race detector (-race).
package goroutinecountermutex

import "sync"

// SafeCounter is a counter safe for concurrent use by multiple goroutines.
type SafeCounter struct {
	mu    sync.Mutex
	count int
}

// Increment adds 1 to the counter in a goroutine-safe way.
func (c *SafeCounter) Increment() {
	panic("TODO: implement Increment")
}

// Value returns the current counter value in a goroutine-safe way.
func (c *SafeCounter) Value() int {
	panic("TODO: implement Value")
}

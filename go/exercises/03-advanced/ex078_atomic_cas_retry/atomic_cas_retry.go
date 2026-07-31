// Package atomiccasretry — Exercise 078 (advanced).
// Goal:   Lock-free update of a shared maximum using a CompareAndSwap retry loop.
// Drills: sync/atomic, optimistic concurrency, CAS retry loops.
package atomiccasretry

import "sync/atomic"

// UpdateMax atomically updates addr to hold the maximum of its current value
// and val, retrying via CompareAndSwap until the update succeeds or is no
// longer necessary. It never blocks and never loses a concurrent update from
// another goroutine.
func UpdateMax(addr *atomic.Int64, val int64) {
	panic("TODO: implement UpdateMax")
}

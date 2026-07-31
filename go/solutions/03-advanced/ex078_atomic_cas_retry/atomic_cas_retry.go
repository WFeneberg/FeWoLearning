// Package atomiccasretry — Exercise 078 (reference solution).
package atomiccasretry

import "sync/atomic"

// UpdateMax atomically updates addr to hold the maximum of its current value
// and val, retrying via CompareAndSwap until the update succeeds or is no
// longer necessary.
func UpdateMax(addr *atomic.Int64, val int64) {
	for {
		old := addr.Load()
		if val <= old {
			return // current value already at least as large; nothing to do
		}
		if addr.CompareAndSwap(old, val) {
			return // swap succeeded: no other goroutine changed it in between
		}
		// Another goroutine updated addr concurrently; reload and retry.
	}
}

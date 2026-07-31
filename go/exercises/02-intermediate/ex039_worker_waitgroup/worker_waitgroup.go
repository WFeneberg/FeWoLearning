// Package workerwaitgroup — Exercise 039 (intermediate).
// Goal:   Sum the ints of several chunks concurrently, one goroutine per
//         chunk, combining partial sums into a shared total.
// Drills: goroutines, sync.WaitGroup, sync.Mutex, shared-state coordination.
package workerwaitgroup

// SumConcurrently spawns one goroutine per chunk in chunks, sums each chunk,
// and accumulates the partial sums into a single total using a
// sync.WaitGroup to wait for completion and a mutex to guard the shared
// total. It returns the sum of all ints across all chunks.
func SumConcurrently(chunks [][]int) int {
	panic("TODO: implement SumConcurrently")
}

// Package workerwaitgroup — Exercise 039 (reference solution).
package workerwaitgroup

import "sync"

// SumConcurrently spawns one goroutine per chunk in chunks, sums each chunk,
// and accumulates the partial sums into a single total using a
// sync.WaitGroup to wait for completion and a mutex to guard the shared
// total. It returns the sum of all ints across all chunks.
func SumConcurrently(chunks [][]int) int {
	var (
		wg    sync.WaitGroup
		mu    sync.Mutex
		total int
	)

	for _, chunk := range chunks {
		wg.Add(1)
		go func(chunk []int) {
			defer wg.Done()

			partial := 0
			for _, v := range chunk {
				partial += v
			}

			mu.Lock()
			total += partial
			mu.Unlock()
		}(chunk)
	}

	wg.Wait()
	return total
}

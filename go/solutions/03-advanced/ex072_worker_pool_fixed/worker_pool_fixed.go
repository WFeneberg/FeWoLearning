// Package workerpoolfixed — Exercise 072 (reference solution).
package workerpoolfixed

import "sync"

// RunPool dispatches each element of jobs to one of workers goroutines,
// applies f to it, and returns the transformed results in a slice whose
// order matches jobs.
func RunPool(jobs []int, workers int, f func(int) int) []int {
	if workers <= 0 {
		panic("workers must be positive")
	}

	results := make([]int, len(jobs))
	if len(jobs) == 0 {
		return results
	}

	type task struct {
		index int
		value int
	}

	tasks := make(chan task)
	var wg sync.WaitGroup

	// Never spawn more workers than there is work to do.
	if workers > len(jobs) {
		workers = len(jobs)
	}

	for i := 0; i < workers; i++ {
		wg.Add(1)
		go func() {
			defer wg.Done()
			for t := range tasks {
				results[t.index] = f(t.value)
			}
		}()
	}

	for i, j := range jobs {
		tasks <- task{index: i, value: j}
	}
	close(tasks)

	wg.Wait()
	return results
}

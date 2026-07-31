// Package workerpoolfixed — Exercise 072 (advanced).
// Goal:   Dispatch a fixed set of jobs across a fixed number of worker
//         goroutines, applying a transform function to each job and
//         collecting the results in the same order as the input.
// Drills: goroutines, channels, sync.WaitGroup, fan-out/fan-in.
package workerpoolfixed

// RunPool dispatches each element of jobs to one of workers goroutines,
// applies f to it, and returns the transformed results in a slice whose
// order matches jobs (results[i] == f(jobs[i])), regardless of which
// worker processed which job or in what order they finished.
//
// It panics if workers <= 0.
func RunPool(jobs []int, workers int, f func(int) int) []int {
	panic("TODO: implement RunPool")
}

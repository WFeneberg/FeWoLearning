// Package fanoutfanin — Exercise 073 (advanced).
// Goal:   Fan work out to several processing goroutines and fan their
//         results back into a single aggregation point.
// Drills: fan-out/fan-in, channels, sync.WaitGroup, goroutine lifecycle.
package fanoutfanin

// Process fans inputs out across workers concurrent goroutines, each of
// which applies work to the values it receives, and fans the results back
// into a single channel that is summed on the caller's goroutine.
//
// It panics if workers <= 0. The order in which inputs are processed is
// unspecified, but the returned sum is always the sum of work(v) for every
// v in inputs.
func Process(inputs []int, workers int, work func(int) int) int {
	panic("TODO: implement Process")
}

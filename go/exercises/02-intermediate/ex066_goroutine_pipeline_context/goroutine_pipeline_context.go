// Package goroutinepipelinecontext — Exercise 066 (intermediate).
// Goal:   build a three-stage goroutine pipeline (generator -> filter -> consumer)
//         where the middle stage stops emitting as soon as its context is
//         canceled, and all pipeline goroutines exit cleanly afterwards.
// Drills: goroutines, channels, context cancellation, select, sync.WaitGroup.
package goroutinepipelinecontext

import "context"

// RunPipeline streams values through a generator -> filter -> consumer
// pipeline. The filter stage forwards values one at a time; after it has
// forwarded cancelAfter values it cancels the internal context, stops
// forwarding any further values, and every stage's goroutine returns.
//
// RunPipeline blocks until the whole pipeline has shut down and returns
// exactly the values the consumer received (in order) before cancellation.
func RunPipeline(ctx context.Context, values []int, cancelAfter int) []int {
	panic("TODO: implement RunPipeline")
}

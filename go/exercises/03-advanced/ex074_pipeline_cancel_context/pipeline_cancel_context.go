// Package pipelinecancelcontext — Exercise 074 (advanced).
// Goal:   A two-stage producer/transform pipeline that stops producing as
//         soon as a shared context is canceled mid-stream, tracking the
//         number of currently-running pipeline goroutines.
// Drills: goroutines, channels, context cancellation, sync.WaitGroup,
//         atomic counters, pipeline shutdown.
package pipelinecancelcontext

import (
	"context"
)

// Stage is a per-item transformation applied by the pipeline's second stage.
type Stage func(int) int

// Pipeline runs a two-stage producer/transform pipeline (producer -> stage1
// channel -> transform -> output channel) and tracks how many pipeline
// goroutines are currently active.
type Pipeline struct {
	// TODO: add fields (active goroutine counter, sync.WaitGroup)
}

// New returns a ready-to-use Pipeline with zero active goroutines.
func New() *Pipeline {
	panic("TODO: implement New")
}

// Run starts the pipeline: a producer goroutine feeds items (in order) into
// an internal channel, and a transform goroutine applies fn to each item
// before writing the result to the returned output channel. Both goroutines
// stop as soon as ctx is canceled — even mid-send — and the output channel
// is closed once both have exited. Items already consumed by the producer
// before cancellation are still delivered in order; no item after the point
// of cancellation is ever produced.
func (p *Pipeline) Run(ctx context.Context, items []int, fn Stage) <-chan int {
	panic("TODO: implement Run")
}

// Wait blocks until every goroutine started by the most recent Run has
// exited.
func (p *Pipeline) Wait() {
	panic("TODO: implement Wait")
}

// Active reports how many pipeline goroutines (producer + transform) are
// currently running. It must reach 0 once Wait returns.
func (p *Pipeline) Active() int {
	panic("TODO: implement Active")
}

// Package pipelinecancelcontext — Exercise 074 (reference solution).
package pipelinecancelcontext

import (
	"context"
	"sync"
	"sync/atomic"
)

// Stage is a per-item transformation applied by the pipeline's second stage.
type Stage func(int) int

// Pipeline runs a two-stage producer/transform pipeline (producer -> stage1
// channel -> transform -> output channel) and tracks how many pipeline
// goroutines are currently active.
type Pipeline struct {
	active int64
	wg     sync.WaitGroup
}

// New returns a ready-to-use Pipeline with zero active goroutines.
func New() *Pipeline {
	return &Pipeline{}
}

// Run starts the pipeline. See the stub for the full contract.
func (p *Pipeline) Run(ctx context.Context, items []int, fn Stage) <-chan int {
	stage1 := make(chan int)
	out := make(chan int)

	p.wg.Add(2)

	// Producer: feeds items into stage1, one at a time, stopping the
	// instant ctx is done — including mid-send.
	atomic.AddInt64(&p.active, 1)
	go func() {
		defer p.wg.Done()
		defer atomic.AddInt64(&p.active, -1)
		defer close(stage1)
		for _, it := range items {
			select {
			case <-ctx.Done():
				return
			case stage1 <- it:
			}
		}
	}()

	// Transform: applies fn to each item read from stage1 and forwards
	// the result to out, again stopping the instant ctx is done.
	atomic.AddInt64(&p.active, 1)
	go func() {
		defer p.wg.Done()
		defer atomic.AddInt64(&p.active, -1)
		defer close(out)
		for {
			select {
			case <-ctx.Done():
				return
			case v, ok := <-stage1:
				if !ok {
					return
				}
				select {
				case <-ctx.Done():
					return
				case out <- fn(v):
				}
			}
		}
	}()

	return out
}

// Wait blocks until every goroutine started by the most recent Run has
// exited.
func (p *Pipeline) Wait() {
	p.wg.Wait()
}

// Active reports how many pipeline goroutines (producer + transform) are
// currently running.
func (p *Pipeline) Active() int {
	return int(atomic.LoadInt64(&p.active))
}

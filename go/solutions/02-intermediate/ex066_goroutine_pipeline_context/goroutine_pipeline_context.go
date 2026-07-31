// Package goroutinepipelinecontext — Exercise 066 (reference solution).
package goroutinepipelinecontext

import (
	"context"
	"sync"
)

// RunPipeline streams values through a generator -> filter -> consumer
// pipeline. The filter stage forwards values one at a time; after it has
// forwarded cancelAfter values it cancels the internal context, stops
// forwarding any further values, and every stage's goroutine returns.
func RunPipeline(ctx context.Context, values []int, cancelAfter int) []int {
	ctx, cancel := context.WithCancel(ctx)
	defer cancel()

	genCh := make(chan int)
	outCh := make(chan int)

	var wg sync.WaitGroup
	wg.Add(2)

	// Stage 1: generator emits values in order, bailing out as soon as the
	// context is canceled instead of blocking forever on a send nobody
	// will receive.
	go func() {
		defer wg.Done()
		defer close(genCh)
		for _, v := range values {
			select {
			case genCh <- v:
			case <-ctx.Done():
				return
			}
		}
	}()

	// Stage 2: filter forwards values downstream until it has forwarded
	// cancelAfter of them, then cancels ctx and stops emitting.
	go func() {
		defer wg.Done()
		defer close(outCh)
		count := 0
		for v := range genCh {
			select {
			case outCh <- v:
			case <-ctx.Done():
				return
			}
			count++
			if count >= cancelAfter {
				cancel()
				return
			}
		}
	}()

	// Stage 3: consumer collects everything the filter forwarded before
	// shutting down.
	var result []int
	for v := range outCh {
		result = append(result, v)
	}

	// Ensure both upstream goroutines have fully exited before returning,
	// so no goroutine is left running/leaked.
	wg.Wait()

	return result
}

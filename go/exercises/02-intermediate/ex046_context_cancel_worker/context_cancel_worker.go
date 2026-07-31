// Package contextcancelworker — Exercise 046 (intermediate).
// Goal:   Run a worker loop that stops promptly when its context is
//         canceled, returning the context's error.
// Drills: context cancellation, select on ctx.Done(), goroutine shutdown.
package contextcancelworker

import "context"

// Worker runs until ctx is canceled (or its deadline expires), then returns
// ctx.Err(). It must not busy-loop the CPU while waiting.
func Worker(ctx context.Context) error {
	panic("TODO: implement Worker")
}

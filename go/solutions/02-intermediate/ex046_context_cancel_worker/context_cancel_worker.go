// Package contextcancelworker — Exercise 046 (reference solution).
package contextcancelworker

import (
	"context"
	"time"
)

// Worker runs until ctx is canceled (or its deadline expires), then returns
// ctx.Err(). It must not busy-loop the CPU while waiting.
func Worker(ctx context.Context) error {
	ticker := time.NewTicker(time.Millisecond)
	defer ticker.Stop()

	for {
		select {
		case <-ctx.Done():
			return ctx.Err()
		case <-ticker.C:
			// simulate doing a small unit of work, then loop again
		}
	}
}

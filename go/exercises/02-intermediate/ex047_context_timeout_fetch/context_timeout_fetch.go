// Package contexttimeoutfetch — Exercise 047 (intermediate).
// Goal:   Simulate slow work that must respect a context deadline.
// Drills: context.WithTimeout, select over ctx.Done() and time.After,
//         propagating context.DeadlineExceeded.
package contexttimeoutfetch

import (
	"context"
	"time"
)

// FetchWithTimeout simulates work that takes `delay` to complete.
// It returns nil if the simulated work finishes before ctx is done,
// or the ctx error (e.g. context.DeadlineExceeded) if the context
// expires first.
func FetchWithTimeout(ctx context.Context, delay time.Duration) error {
	panic("TODO: implement FetchWithTimeout")
}

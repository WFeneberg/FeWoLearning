// Package contexttimeoutfetch — Exercise 047 (reference solution).
package contexttimeoutfetch

import (
	"context"
	"time"
)

func FetchWithTimeout(ctx context.Context, delay time.Duration) error {
	timer := time.NewTimer(delay)
	defer timer.Stop()

	select {
	case <-ctx.Done():
		return ctx.Err()
	case <-timer.C:
		return nil
	}
}

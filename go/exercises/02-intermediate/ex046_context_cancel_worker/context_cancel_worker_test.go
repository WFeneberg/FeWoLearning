package contextcancelworker

import (
	"context"
	"errors"
	"testing"
	"time"
)

func TestWorker(t *testing.T) {
	cases := []struct {
		name string
	}{
		{name: "cancel_after_start"},
		{name: "already_canceled"},
	}

	for _, tc := range cases {
		t.Run(tc.name, func(t *testing.T) {
			ctx, cancel := context.WithCancel(context.Background())
			// Guarantees the context is released even on the timeout path.
			// cancel is idempotent, so the explicit calls below stay valid.
			defer cancel()

			if tc.name == "already_canceled" {
				cancel()
			}

			done := make(chan error, 1)
			go func() {
				done <- Worker(ctx)
			}()

			if tc.name == "cancel_after_start" {
				cancel()
			}

			select {
			case err := <-done:
				if !errors.Is(err, context.Canceled) {
					t.Fatalf("Worker returned err = %v, want context.Canceled", err)
				}
			case <-time.After(time.Second):
				t.Fatal("Worker did not return promptly after context cancellation")
			}
		})
	}
}

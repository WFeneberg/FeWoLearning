package contexttimeoutfetch

import (
	"context"
	"errors"
	"testing"
	"time"
)

func TestFetchWithTimeout(t *testing.T) {
	cases := []struct {
		name      string
		timeout   time.Duration
		delay     time.Duration
		wantErr   error
		wantIsNil bool
	}{
		{name: "completes before deadline", timeout: 100 * time.Millisecond, delay: 5 * time.Millisecond, wantIsNil: true},
		{name: "exceeds deadline", timeout: 5 * time.Millisecond, delay: 100 * time.Millisecond, wantErr: context.DeadlineExceeded},
	}

	for _, tc := range cases {
		tc := tc
		t.Run(tc.name, func(t *testing.T) {
			ctx, cancel := context.WithTimeout(context.Background(), tc.timeout)
			defer cancel()

			err := FetchWithTimeout(ctx, tc.delay)

			if tc.wantIsNil {
				if err != nil {
					t.Fatalf("FetchWithTimeout() = %v, want nil", err)
				}
				return
			}

			if !errors.Is(err, tc.wantErr) {
				t.Fatalf("FetchWithTimeout() = %v, want %v", err, tc.wantErr)
			}
		})
	}
}

func TestFetchWithTimeout_AlreadyCanceled(t *testing.T) {
	ctx, cancel := context.WithCancel(context.Background())
	cancel()

	err := FetchWithTimeout(ctx, 10*time.Millisecond)
	if !errors.Is(err, context.Canceled) {
		t.Fatalf("FetchWithTimeout() with pre-canceled ctx = %v, want %v", err, context.Canceled)
	}
}

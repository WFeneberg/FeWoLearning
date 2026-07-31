package loadsheddingbackpressure

import (
	"errors"
	"sync"
	"sync/atomic"
	"testing"
)

func TestSubmitAcceptsWithinCapacityThenSheds(t *testing.T) {
	s := NewServer(3)
	for i := 0; i < 3; i++ {
		if err := s.Submit(); err != nil {
			t.Fatalf("Submit #%d: unexpected error %v", i, err)
		}
	}
	if got := s.QueueLen(); got != 3 {
		t.Fatalf("QueueLen = %d, want 3", got)
	}
	if err := s.Submit(); !errors.Is(err, ErrQueueFull) {
		t.Fatalf("Submit on full queue = %v, want ErrQueueFull", err)
	}

	// Completing a request frees a slot for exactly one more Submit.
	s.Complete()
	if got := s.QueueLen(); got != 2 {
		t.Fatalf("QueueLen after Complete = %d, want 2", got)
	}
	if err := s.Submit(); err != nil {
		t.Fatalf("Submit after Complete: unexpected error %v", err)
	}
	if got := s.QueueLen(); got != 3 {
		t.Fatalf("QueueLen = %d, want 3", got)
	}
	if err := s.Submit(); !errors.Is(err, ErrQueueFull) {
		t.Fatalf("Submit on re-full queue = %v, want ErrQueueFull", err)
	}
}

func TestSubmitTableDriven(t *testing.T) {
	cases := []struct {
		name     string
		capacity int
		submits  int
		wantOK   int
	}{
		{"exact fit", 4, 4, 4},
		{"over by one", 2, 3, 2},
		{"way over capacity", 1, 10, 1},
		{"under capacity", 5, 2, 2},
	}
	for _, tc := range cases {
		t.Run(tc.name, func(t *testing.T) {
			s := NewServer(tc.capacity)
			accepted := 0
			for i := 0; i < tc.submits; i++ {
				err := s.Submit()
				switch {
				case err == nil:
					accepted++
				case errors.Is(err, ErrQueueFull):
					// expected once saturated
				default:
					t.Fatalf("Submit returned unexpected error: %v", err)
				}
			}
			if accepted != tc.wantOK {
				t.Errorf("accepted = %d, want %d", accepted, tc.wantOK)
			}
			if got := s.QueueLen(); got != tc.wantOK {
				t.Errorf("QueueLen = %d, want %d", got, tc.wantOK)
			}
			if s.Capacity() != tc.capacity {
				t.Errorf("Capacity = %d, want %d", s.Capacity(), tc.capacity)
			}
		})
	}
}

// TestConcurrentSubmitShedsExcessDeterministically hammers Submit from many
// goroutines at once. Even though goroutine scheduling is nondeterministic,
// the *outcome* is fully deterministic: exactly `capacity` submissions must
// succeed and every remaining one must be shed with ErrQueueFull, because
// admission control must never over-admit past the configured capacity.
func TestConcurrentSubmitShedsExcessDeterministically(t *testing.T) {
	const capacity = 10
	const workers = 200
	s := NewServer(capacity)

	var accepted, rejected, otherErr int64
	var wg sync.WaitGroup
	wg.Add(workers)
	for i := 0; i < workers; i++ {
		go func() {
			defer wg.Done()
			switch err := s.Submit(); {
			case err == nil:
				atomic.AddInt64(&accepted, 1)
			case errors.Is(err, ErrQueueFull):
				atomic.AddInt64(&rejected, 1)
			default:
				atomic.AddInt64(&otherErr, 1)
			}
		}()
	}
	wg.Wait()

	if otherErr != 0 {
		t.Fatalf("got %d unexpected (non-ErrQueueFull) errors from Submit", otherErr)
	}
	if accepted != capacity {
		t.Errorf("accepted = %d, want %d", accepted, capacity)
	}
	if rejected != workers-capacity {
		t.Errorf("rejected = %d, want %d", rejected, workers-capacity)
	}
	if got := s.QueueLen(); got != capacity {
		t.Errorf("QueueLen = %d, want %d", got, capacity)
	}

	// Freeing every accepted slot must bring the queue back to empty, and
	// admission must reopen for exactly `capacity` more requests.
	for i := 0; i < capacity; i++ {
		s.Complete()
	}
	if got := s.QueueLen(); got != 0 {
		t.Fatalf("QueueLen after draining = %d, want 0", got)
	}
	reopened := 0
	for i := 0; i < workers; i++ {
		if err := s.Submit(); err == nil {
			reopened++
		}
	}
	if reopened != capacity {
		t.Errorf("reopened accepted = %d, want %d", reopened, capacity)
	}
}

func TestCompleteOnEmptyQueuePanics(t *testing.T) {
	s := NewServer(1)
	defer func() {
		if recover() == nil {
			t.Fatal("expected Complete on empty queue to panic")
		}
	}()
	s.Complete()
}

func TestNewServerPanicsOnNonPositiveCapacity(t *testing.T) {
	for _, capacity := range []int{0, -1, -5} {
		func() {
			defer func() {
				if recover() == nil {
					t.Errorf("NewServer(%d): expected panic", capacity)
				}
			}()
			NewServer(capacity)
		}()
	}
}

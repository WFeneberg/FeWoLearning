package connectionpool

import (
	"runtime"
	"sync"
	"testing"
)

// TestNeverExceedsSize drives more concurrent Acquire/Release cycles than the
// pool's size through the pool and asserts that the number of distinct
// Connections ever created never exceeds size.
func TestNeverExceedsSize(t *testing.T) {
	cases := []struct {
		name    string
		size    int
		workers int
	}{
		{"size1", 1, 8},
		{"size2", 2, 12},
		{"size4", 4, 20},
	}

	for _, tc := range cases {
		t.Run(tc.name, func(t *testing.T) {
			p := NewPool(tc.size)

			var wg sync.WaitGroup
			var mu sync.Mutex
			seen := make(map[int]bool)

			for i := 0; i < tc.workers; i++ {
				wg.Add(1)
				go func() {
					defer wg.Done()
					c := p.Acquire()
					mu.Lock()
					seen[c.ID] = true
					mu.Unlock()
					p.Release(c)
				}()
			}
			wg.Wait()

			if got := p.Created(); got > tc.size {
				t.Fatalf("Created() = %d, want <= %d", got, tc.size)
			}
			if len(seen) > tc.size {
				t.Fatalf("saw %d distinct connection IDs, want <= %d", len(seen), tc.size)
			}
		})
	}
}

// TestReleaseReusesConnection checks that a Connection handed back via
// Release is the one returned by the next Acquire, and that no additional
// Connection was created for it.
func TestReleaseReusesConnection(t *testing.T) {
	p := NewPool(1)

	c1 := p.Acquire()
	if got := p.Created(); got != 1 {
		t.Fatalf("Created() = %d, want 1", got)
	}

	p.Release(c1)

	c2 := p.Acquire()
	if c2 != c1 {
		t.Fatalf("Acquire() after Release returned a different Connection: got ID %d, want ID %d", c2.ID, c1.ID)
	}
	if got := p.Created(); got != 1 {
		t.Fatalf("Created() = %d, want 1 (no new connection should have been made)", got)
	}
}

// TestAcquireBlocksUntilRelease exhausts a size-1 pool and confirms that a
// second Acquire call, running in its own goroutine, does not return until
// the checked-out Connection is Released. It relies only on goroutine
// scheduling and channel synchronization (no timers, sleeps, or real-time
// waits), so the assertions are deterministic.
func TestAcquireBlocksUntilRelease(t *testing.T) {
	p := NewPool(1)

	c1 := p.Acquire()

	result := make(chan *Connection, 1)
	go func() {
		result <- p.Acquire()
	}()

	// Give the goroutine every opportunity to run. If Acquire were
	// (incorrectly) non-blocking, it would have sent on result by now.
	for i := 0; i < 10000; i++ {
		runtime.Gosched()
	}

	select {
	case c := <-result:
		t.Fatalf("Acquire() returned %v before Release was called; want it to block while pool is exhausted", c)
	default:
		// Expected: still blocked.
	}

	p.Release(c1)

	// Now that a Connection was released, the blocked Acquire must
	// complete. This receive blocks until it does (or the test's own
	// timeout fires), so it needs no wall-clock wait of its own.
	c2 := <-result
	if c2 != c1 {
		t.Fatalf("unblocked Acquire() returned ID %d, want the released ID %d", c2.ID, c1.ID)
	}
	if got := p.Created(); got != 1 {
		t.Fatalf("Created() = %d, want 1", got)
	}
}

package atomiccounter

import (
	"sync"
	"testing"
)

func TestIncrementSequential(t *testing.T) {
	c := NewAtomicCounter()
	if got := c.Load(); got != 0 {
		t.Fatalf("Load() on new counter = %d, want 0", got)
	}
	for i := 1; i <= 5; i++ {
		if got := c.Increment(); got != int64(i) {
			t.Errorf("Increment() call #%d = %d, want %d", i, got, i)
		}
	}
	if got := c.Load(); got != 5 {
		t.Errorf("Load() = %d, want 5", got)
	}
}

func TestAddNegative(t *testing.T) {
	c := NewAtomicCounter()
	c.Add(10)
	if got := c.Add(-3); got != 7 {
		t.Errorf("Add(-3) = %d, want 7", got)
	}
	if got := c.Load(); got != 7 {
		t.Errorf("Load() = %d, want 7", got)
	}
}

// TestConcurrentIncrement launches many goroutines that each perform a fixed
// number of increments. Because AtomicCounter must be lock-free but still
// correct, the final value must equal the exact total of all increments —
// no lost updates, regardless of goroutine interleaving.
func TestConcurrentIncrement(t *testing.T) {
	const goroutines = 200
	const perGoroutine = 500
	const want = int64(goroutines * perGoroutine)

	c := NewAtomicCounter()

	var wg sync.WaitGroup
	wg.Add(goroutines)
	for i := 0; i < goroutines; i++ {
		go func() {
			defer wg.Done()
			for j := 0; j < perGoroutine; j++ {
				c.Increment()
			}
		}()
	}
	wg.Wait()

	if got := c.Load(); got != want {
		t.Errorf("Load() after concurrent increments = %d, want %d", got, want)
	}
}

// TestConcurrentAddMixed mixes positive and negative deltas across many
// goroutines; the net result is deterministic (zero) even though the
// interleaving of individual Add calls is not.
func TestConcurrentAddMixed(t *testing.T) {
	const goroutines = 100
	const perGoroutine = 300

	c := NewAtomicCounter()
	c.Add(1_000_000) // baseline so we can subtract it back to zero exactly

	var wg sync.WaitGroup
	wg.Add(goroutines * 2)
	for i := 0; i < goroutines; i++ {
		go func() {
			defer wg.Done()
			for j := 0; j < perGoroutine; j++ {
				c.Add(1)
			}
		}()
		go func() {
			defer wg.Done()
			for j := 0; j < perGoroutine; j++ {
				c.Add(-1)
			}
		}()
	}
	wg.Wait()

	if got := c.Load(); got != 1_000_000 {
		t.Errorf("Load() after balanced concurrent adds = %d, want 1000000", got)
	}
}

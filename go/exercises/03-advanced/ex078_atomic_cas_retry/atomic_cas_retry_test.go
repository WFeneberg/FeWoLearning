package atomiccasretry

import (
	"sync"
	"sync/atomic"
	"testing"
)

func TestUpdateMaxSequential(t *testing.T) {
	var addr atomic.Int64
	addr.Store(10)

	UpdateMax(&addr, 5) // lower than current: no-op
	if got := addr.Load(); got != 10 {
		t.Fatalf("after lower update: addr = %d, want 10", got)
	}

	UpdateMax(&addr, 42) // higher than current: updates
	if got := addr.Load(); got != 42 {
		t.Fatalf("after higher update: addr = %d, want 42", got)
	}

	UpdateMax(&addr, 42) // equal: stays the same
	if got := addr.Load(); got != 42 {
		t.Fatalf("after equal update: addr = %d, want 42", got)
	}
}

func TestUpdateMaxNegativeInitial(t *testing.T) {
	var addr atomic.Int64
	addr.Store(-100)

	UpdateMax(&addr, -50)
	if got := addr.Load(); got != -50 {
		t.Fatalf("addr = %d, want -50", got)
	}

	UpdateMax(&addr, -200)
	if got := addr.Load(); got != -50 {
		t.Fatalf("addr = %d, want -50 (unchanged)", got)
	}
}

// TestUpdateMaxConcurrent races many goroutines against UpdateMax with a
// fixed, known set of values. Regardless of scheduling/interleaving, the
// final stored value must equal the true maximum of the input set, and no
// update may ever be silently lost.
func TestUpdateMaxConcurrent(t *testing.T) {
	const workers = 200
	var addr atomic.Int64
	addr.Store(0)

	// Deterministic set of candidate values; true max is workers*7 = 1400.
	values := make([]int64, workers)
	wantMax := int64(0)
	for i := 0; i < workers; i++ {
		v := int64(i) * 7
		values[i] = v
		if v > wantMax {
			wantMax = v
		}
	}

	var wg sync.WaitGroup
	wg.Add(workers)
	for i := 0; i < workers; i++ {
		v := values[i]
		go func() {
			defer wg.Done()
			UpdateMax(&addr, v)
		}()
	}
	wg.Wait()

	if got := addr.Load(); got != wantMax {
		t.Fatalf("after concurrent updates: addr = %d, want %d", got, wantMax)
	}
}

// TestUpdateMaxConcurrentMixedSigns exercises negative and positive values
// racing together to guard against implementations that special-case zero
// or assume monotonically increasing input.
func TestUpdateMaxConcurrentMixedSigns(t *testing.T) {
	var addr atomic.Int64
	addr.Store(-1_000_000)

	values := []int64{-5, 3, -100, 999, 998, -1, 0, 1000, 500, -999}
	wantMax := int64(1000)

	var wg sync.WaitGroup
	wg.Add(len(values))
	for _, v := range values {
		v := v
		go func() {
			defer wg.Done()
			UpdateMax(&addr, v)
		}()
	}
	wg.Wait()

	if got := addr.Load(); got != wantMax {
		t.Fatalf("addr = %d, want %d", got, wantMax)
	}
}

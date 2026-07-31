package concurrentkvstore

import (
	"fmt"
	"sync"
	"testing"
)

func TestNewPanicsOnInvalidShardCount(t *testing.T) {
	defer func() {
		if recover() == nil {
			t.Fatal("expected New(0) to panic")
		}
	}()
	New[int](0)
}

func TestSetGetDeleteSequential(t *testing.T) {
	s := New[int](4)

	if _, ok := s.Get("missing"); ok {
		t.Fatal("expected missing key to be absent")
	}

	s.Set("a", 1)
	s.Set("b", 2)
	if v, ok := s.Get("a"); !ok || v != 1 {
		t.Errorf("Get(a) = %d,%v want 1,true", v, ok)
	}
	if v, ok := s.Get("b"); !ok || v != 2 {
		t.Errorf("Get(b) = %d,%v want 2,true", v, ok)
	}
	if s.Len() != 2 {
		t.Errorf("Len() = %d want 2", s.Len())
	}

	s.Set("a", 10) // overwrite
	if v, _ := s.Get("a"); v != 10 {
		t.Errorf("Get(a) after overwrite = %d want 10", v)
	}
	if s.Len() != 2 {
		t.Errorf("Len() after overwrite = %d want 2", s.Len())
	}

	s.Delete("a")
	if _, ok := s.Get("a"); ok {
		t.Error("expected 'a' deleted")
	}
	if s.Len() != 1 {
		t.Errorf("Len() after delete = %d want 1", s.Len())
	}

	s.Delete("does-not-exist") // no-op, must not panic
	if s.Len() != 1 {
		t.Errorf("Len() after no-op delete = %d want 1", s.Len())
	}
}

func TestShardDistribution(t *testing.T) {
	s := New[int](8)
	if s.NumShards() != 8 {
		t.Fatalf("NumShards() = %d want 8", s.NumShards())
	}

	seen := make(map[int]bool)
	for i := 0; i < 64; i++ {
		key := fmt.Sprintf("key-%d", i)
		idx := s.ShardIndex(key)
		if idx < 0 || idx >= 8 {
			t.Fatalf("ShardIndex(%q) = %d out of range [0,8)", key, idx)
		}
		// ShardIndex must be a pure function of the key.
		if idx2 := s.ShardIndex(key); idx2 != idx {
			t.Fatalf("ShardIndex(%q) not deterministic: %d then %d", key, idx, idx2)
		}
		seen[idx] = true
	}
	if len(seen) < 2 {
		t.Errorf("expected keys spread across multiple shards, all landed in %v", seen)
	}
}

// TestConcurrentDistinctKeys writes N distinct keys from N goroutines
// concurrently, then verifies every key round-trips correctly. Run with
// `go test -race` to confirm the shard locking is actually race-free.
func TestConcurrentDistinctKeys(t *testing.T) {
	const n = 500
	s := New[int](16)

	var wg sync.WaitGroup
	wg.Add(n)
	for i := 0; i < n; i++ {
		i := i
		go func() {
			defer wg.Done()
			s.Set(fmt.Sprintf("k%d", i), i*i)
		}()
	}
	wg.Wait()

	if got := s.Len(); got != n {
		t.Fatalf("Len() = %d want %d", got, n)
	}
	for i := 0; i < n; i++ {
		want := i * i
		v, ok := s.Get(fmt.Sprintf("k%d", i))
		if !ok || v != want {
			t.Errorf("Get(k%d) = %d,%v want %d,true", i, v, ok, want)
		}
	}
}

// TestConcurrentOverlappingKeysSameValue hammers a small, fixed set of keys
// from many goroutines that all write the same value for a given key. Since
// every writer of a given key agrees on the value, the final state is
// deterministic regardless of goroutine scheduling, letting the test assert
// exact results while still exercising real write contention on shared
// shards.
func TestConcurrentOverlappingKeysSameValue(t *testing.T) {
	const numKeys = 4
	const writersPerKey = 200
	s := New[int](4)

	keyFor := func(k int) string { return fmt.Sprintf("shared-%d", k) }
	valueFor := func(k int) int { return (k + 1) * 7 }

	var wg sync.WaitGroup
	wg.Add(numKeys * writersPerKey)
	for k := 0; k < numKeys; k++ {
		k := k
		for w := 0; w < writersPerKey; w++ {
			go func() {
				defer wg.Done()
				s.Set(keyFor(k), valueFor(k))
			}()
		}
	}
	wg.Wait()

	if got := s.Len(); got != numKeys {
		t.Fatalf("Len() = %d want %d", got, numKeys)
	}
	for k := 0; k < numKeys; k++ {
		v, ok := s.Get(keyFor(k))
		if !ok || v != valueFor(k) {
			t.Errorf("Get(%s) = %d,%v want %d,true", keyFor(k), v, ok, valueFor(k))
		}
	}
}

// TestConcurrentSetThenDelete sets N distinct keys concurrently, waits for
// all writers to finish, then deletes all N keys concurrently and checks the
// store ends up empty. Exercises interleaved lock acquisition across
// multiple shards from many goroutines simultaneously.
func TestConcurrentSetThenDelete(t *testing.T) {
	const n = 300
	s := New[int](8)

	var wg sync.WaitGroup
	wg.Add(n)
	for i := 0; i < n; i++ {
		i := i
		go func() {
			defer wg.Done()
			s.Set(fmt.Sprintf("d%d", i), i)
		}()
	}
	wg.Wait()

	if got := s.Len(); got != n {
		t.Fatalf("Len() after inserts = %d want %d", got, n)
	}

	wg.Add(n)
	for i := 0; i < n; i++ {
		i := i
		go func() {
			defer wg.Done()
			s.Delete(fmt.Sprintf("d%d", i))
		}()
	}
	wg.Wait()

	if got := s.Len(); got != 0 {
		t.Fatalf("Len() after deletes = %d want 0", got)
	}
	for i := 0; i < n; i++ {
		if _, ok := s.Get(fmt.Sprintf("d%d", i)); ok {
			t.Fatalf("Get(d%d) still present after concurrent delete", i)
		}
	}
}

// TestConcurrentReadWriteMix runs concurrent readers and writers against an
// overlapping set of keys at the same time (rather than in separate waves)
// to make sure Get/Set/Delete can be safely invoked concurrently without
// data races. Correctness here is defined as "no race and no panic"; final
// values are additionally checked against the last logical writer group.
func TestConcurrentReadWriteMix(t *testing.T) {
	const numKeys = 10
	const rounds = 100
	s := New[int](4)

	key := func(k int) string { return fmt.Sprintf("mix-%d", k) }

	// Seed all keys first so readers always see a present key.
	for k := 0; k < numKeys; k++ {
		s.Set(key(k), -1)
	}

	var wg sync.WaitGroup

	// Writers: each key k is only ever written the value `rounds-1` by its
	// last writer in program order per goroutine set, but since multiple
	// goroutines write the same key with the same final target value, the
	// end state is deterministic: every key ends at value `rounds - 1`.
	for k := 0; k < numKeys; k++ {
		k := k
		wg.Add(1)
		go func() {
			defer wg.Done()
			for r := 0; r < rounds; r++ {
				s.Set(key(k), r)
			}
		}()
	}

	// Readers run concurrently purely to exercise the RWMutex read path;
	// their results are not asserted since the writers are still racing
	// with them by design (only checked for absence of panics/races).
	for k := 0; k < numKeys; k++ {
		k := k
		wg.Add(1)
		go func() {
			defer wg.Done()
			for r := 0; r < rounds; r++ {
				s.Get(key(k))
			}
		}()
	}

	wg.Wait()

	if got := s.Len(); got != numKeys {
		t.Fatalf("Len() = %d want %d", got, numKeys)
	}
	for k := 0; k < numKeys; k++ {
		v, ok := s.Get(key(k))
		if !ok || v != rounds-1 {
			t.Errorf("Get(%s) = %d,%v want %d,true", key(k), v, ok, rounds-1)
		}
	}
}

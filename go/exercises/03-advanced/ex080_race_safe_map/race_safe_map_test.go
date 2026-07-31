package racesafemap

import (
	"sync"
	"testing"
)

// TestSequentialSetGetDeleteLen exercises the basic API with no concurrency,
// table-driven over a sequence of operations.
func TestSequentialSetGetDeleteLen(t *testing.T) {
	m := New[string, int]()

	type op struct {
		name       string
		key        string
		set        bool
		value      int
		del        bool
		wantGet    int
		wantOK     bool
		wantLenAft int
	}
	ops := []op{
		{name: "set a=1", key: "a", set: true, value: 1, wantGet: 1, wantOK: true, wantLenAft: 1},
		{name: "set b=2", key: "b", set: true, value: 2, wantGet: 2, wantOK: true, wantLenAft: 2},
		{name: "overwrite a=10", key: "a", set: true, value: 10, wantGet: 10, wantOK: true, wantLenAft: 2},
		{name: "get missing c", key: "c", wantGet: 0, wantOK: false, wantLenAft: 2},
		{name: "delete b", key: "b", del: true, wantGet: 0, wantOK: false, wantLenAft: 1},
	}

	for _, o := range ops {
		if o.set {
			m.Set(o.key, o.value)
		}
		if o.del {
			m.Delete(o.key)
		}
		got, ok := m.Get(o.key)
		if got != o.wantGet || ok != o.wantOK {
			t.Errorf("%s: Get(%q) = (%d, %v), want (%d, %v)", o.name, o.key, got, ok, o.wantGet, o.wantOK)
		}
		if gotLen := m.Len(); gotLen != o.wantLenAft {
			t.Errorf("%s: Len() = %d, want %d", o.name, gotLen, o.wantLenAft)
		}
	}
}

// TestConcurrentUpdateIsAtomic hammers a small set of keys from many
// goroutines using Update as a read-modify-write increment. If Update does
// not hold the lock across the whole read-modify-write, this test loses
// increments (wrong final totals) and, under `go test -race`, is reported as
// a data race.
func TestConcurrentUpdateIsAtomic(t *testing.T) {
	m := New[string, int]()
	const goroutines = 50
	const incrementsEach = 200
	keys := []string{"alpha", "beta", "gamma"}

	var wg sync.WaitGroup
	for _, k := range keys {
		k := k
		for g := 0; g < goroutines; g++ {
			wg.Add(1)
			go func() {
				defer wg.Done()
				for i := 0; i < incrementsEach; i++ {
					m.Update(k, func(v int) int { return v + 1 })
				}
			}()
		}
	}
	wg.Wait()

	want := goroutines * incrementsEach
	for _, k := range keys {
		got, ok := m.Get(k)
		if !ok || got != want {
			t.Errorf("Get(%q) = (%d, %v), want (%d, true)", k, got, ok, want)
		}
	}
	if gotLen := m.Len(); gotLen != len(keys) {
		t.Errorf("Len() = %d, want %d", gotLen, len(keys))
	}
}

// TestConcurrentSetGetDeleteDistinctKeys exercises Set/Get/Delete from many
// goroutines operating on distinct keys concurrently with readers polling
// throughout, catching any missing synchronization under -race.
func TestConcurrentSetGetDeleteDistinctKeys(t *testing.T) {
	m := New[int, int]()
	const n = 500

	var wg sync.WaitGroup
	for i := 0; i < n; i++ {
		i := i
		wg.Add(1)
		go func() {
			defer wg.Done()
			m.Set(i, i*i)
		}()
	}
	// Concurrent readers/length checks while writers are still running.
	for i := 0; i < 20; i++ {
		wg.Add(1)
		go func() {
			defer wg.Done()
			_ = m.Len()
			_, _ = m.Get(0)
		}()
	}
	wg.Wait()

	if gotLen := m.Len(); gotLen != n {
		t.Fatalf("Len() = %d, want %d", gotLen, n)
	}
	for i := 0; i < n; i++ {
		got, ok := m.Get(i)
		if !ok || got != i*i {
			t.Errorf("Get(%d) = (%d, %v), want (%d, true)", i, got, ok, i*i)
		}
	}

	var wg2 sync.WaitGroup
	for i := 0; i < n; i += 2 {
		i := i
		wg2.Add(1)
		go func() {
			defer wg2.Done()
			m.Delete(i)
		}()
	}
	wg2.Wait()

	if gotLen := m.Len(); gotLen != n/2 {
		t.Fatalf("after deletes: Len() = %d, want %d", gotLen, n/2)
	}
	for i := 1; i < n; i += 2 {
		got, ok := m.Get(i)
		if !ok || got != i*i {
			t.Errorf("Get(%d) = (%d, %v), want (%d, true)", i, got, ok, i*i)
		}
	}
	for i := 0; i < n; i += 2 {
		if _, ok := m.Get(i); ok {
			t.Errorf("Get(%d) present, want deleted", i)
		}
	}
}

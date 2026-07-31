package rwmutexcache

import (
	"strconv"
	"sync"
	"testing"
)

func TestCacheBasicOps(t *testing.T) {
	cases := []struct {
		name string
		run  func(c *Cache) (int, bool)
	}{
		{"missing key", func(c *Cache) (int, bool) {
			return c.Get("nope")
		}},
		{"set then get", func(c *Cache) (int, bool) {
			c.Set("a", 42)
			return c.Get("a")
		}},
		{"overwrite", func(c *Cache) (int, bool) {
			c.Set("a", 1)
			c.Set("a", 2)
			return c.Get("a")
		}},
		{"delete", func(c *Cache) (int, bool) {
			c.Set("a", 1)
			c.Delete("a")
			return c.Get("a")
		}},
	}

	wants := []struct {
		val int
		ok  bool
	}{
		{0, false},
		{42, true},
		{2, true},
		{0, false},
	}

	for i, tc := range cases {
		c := NewCache()
		gotVal, gotOK := tc.run(c)
		if gotVal != wants[i].val || gotOK != wants[i].ok {
			t.Errorf("%s: got (%d, %v), want (%d, %v)", tc.name, gotVal, gotOK, wants[i].val, wants[i].ok)
		}
	}
}

func TestCacheLen(t *testing.T) {
	c := NewCache()
	if got := c.Len(); got != 0 {
		t.Fatalf("Len() on empty cache = %d, want 0", got)
	}
	c.Set("x", 1)
	c.Set("y", 2)
	c.Set("x", 3) // overwrite, should not increase length
	if got := c.Len(); got != 2 {
		t.Fatalf("Len() = %d, want 2", got)
	}
	c.Delete("x")
	if got := c.Len(); got != 1 {
		t.Fatalf("Len() after delete = %d, want 1", got)
	}
}

// TestCacheConcurrentAccess exercises concurrent Set/Get from many
// goroutines to ensure RWMutex protects the map from data races
// (run with `go test -race`) and that the final state is correct.
func TestCacheConcurrentAccess(t *testing.T) {
	const numKeys = 50
	const writersPerKey = 4

	c := NewCache()
	var wg sync.WaitGroup

	// Concurrent writers: each key is written by several goroutines,
	// with the last logical writer (highest value) determining the
	// final value for that key.
	for k := 0; k < numKeys; k++ {
		key := strconv.Itoa(k)
		for w := 0; w < writersPerKey; w++ {
			wg.Add(1)
			go func(key string, value int) {
				defer wg.Done()
				c.Set(key, value)
			}(key, w)
		}
	}

	// Concurrent readers running alongside the writers.
	for r := 0; r < numKeys; r++ {
		key := strconv.Itoa(r)
		wg.Add(1)
		go func(key string) {
			defer wg.Done()
			c.Get(key) // result not asserted here; just exercising RLock concurrently
		}(key)
	}

	wg.Wait()

	if got := c.Len(); got != numKeys {
		t.Fatalf("Len() after concurrent writes = %d, want %d", got, numKeys)
	}

	// Every key must be present with one of the values written by its writers.
	for k := 0; k < numKeys; k++ {
		key := strconv.Itoa(k)
		v, ok := c.Get(key)
		if !ok {
			t.Fatalf("key %q missing after concurrent writes", key)
		}
		if v < 0 || v >= writersPerKey {
			t.Fatalf("key %q = %d, want value in [0,%d)", key, v, writersPerKey)
		}
	}
}

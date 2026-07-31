// Package racesafemap — Exercise 080 (advanced).
// Goal:   Fix a racy generic map wrapper by adding correct locking so that
//         concurrent reads, writes, and read-modify-write updates are safe.
//         Run tests with `go test -race` to confirm no data race is reported.
// Drills: race debugging, sync.RWMutex, read-modify-write atomicity.
package racesafemap

import "sync"

// RaceSafeMap is a generic map wrapper intended to be safe for concurrent
// use by multiple goroutines. The zero value is NOT ready to use; call New.
type RaceSafeMap[K comparable, V any] struct {
	mu   sync.RWMutex
	data map[K]V
}

// New creates an empty RaceSafeMap.
func New[K comparable, V any]() *RaceSafeMap[K, V] {
	panic("TODO: implement New")
}

// Set stores value under key, overwriting any existing value.
func (m *RaceSafeMap[K, V]) Set(key K, value V) {
	panic("TODO: implement Set")
}

// Get returns the value stored under key and whether it was present.
func (m *RaceSafeMap[K, V]) Get(key K) (V, bool) {
	panic("TODO: implement Get")
}

// Delete removes key from the map, if present.
func (m *RaceSafeMap[K, V]) Delete(key K) {
	panic("TODO: implement Delete")
}

// Len returns the number of entries currently stored.
func (m *RaceSafeMap[K, V]) Len() int {
	panic("TODO: implement Len")
}

// Update atomically reads the current value for key (the zero value if
// absent), applies fn to compute the new value, stores it, and returns it.
// The read, compute, and write must happen as a single atomic operation with
// respect to other goroutines calling Update/Set/Get/Delete concurrently.
func (m *RaceSafeMap[K, V]) Update(key K, fn func(V) V) V {
	panic("TODO: implement Update")
}

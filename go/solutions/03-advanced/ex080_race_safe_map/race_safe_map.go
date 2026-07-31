// Package racesafemap — Exercise 080 (reference solution).
package racesafemap

import "sync"

// RaceSafeMap is a generic map wrapper safe for concurrent use by multiple
// goroutines. The zero value is NOT ready to use; call New.
type RaceSafeMap[K comparable, V any] struct {
	mu   sync.RWMutex
	data map[K]V
}

// New creates an empty RaceSafeMap.
func New[K comparable, V any]() *RaceSafeMap[K, V] {
	return &RaceSafeMap[K, V]{data: make(map[K]V)}
}

// Set stores value under key, overwriting any existing value.
func (m *RaceSafeMap[K, V]) Set(key K, value V) {
	m.mu.Lock()
	defer m.mu.Unlock()
	m.data[key] = value
}

// Get returns the value stored under key and whether it was present.
func (m *RaceSafeMap[K, V]) Get(key K) (V, bool) {
	m.mu.RLock()
	defer m.mu.RUnlock()
	v, ok := m.data[key]
	return v, ok
}

// Delete removes key from the map, if present.
func (m *RaceSafeMap[K, V]) Delete(key K) {
	m.mu.Lock()
	defer m.mu.Unlock()
	delete(m.data, key)
}

// Len returns the number of entries currently stored.
func (m *RaceSafeMap[K, V]) Len() int {
	m.mu.RLock()
	defer m.mu.RUnlock()
	return len(m.data)
}

// Update atomically reads the current value for key (the zero value if
// absent), applies fn to compute the new value, stores it, and returns it.
// Holding the write lock across the whole read-modify-write is what makes
// this safe under concurrent callers; a Get+compute+Set sequence without a
// single held lock would lose updates (and race).
func (m *RaceSafeMap[K, V]) Update(key K, fn func(V) V) V {
	m.mu.Lock()
	defer m.mu.Unlock()
	nv := fn(m.data[key])
	m.data[key] = nv
	return nv
}

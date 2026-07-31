// Package lrucache — Exercise 071 (advanced).
// Goal:   Fixed-capacity LRU cache with O(1) Get/Put using container/list + map.
// Drills: generics, container/list, eviction policy.
package lrucache

// Cache is a generic fixed-capacity LRU cache.
type Cache[K comparable, V any] struct {
	// TODO: add fields (capacity, map, list)
}

// New creates a cache. It panics if capacity <= 0.
func New[K comparable, V any](capacity int) *Cache[K, V] {
	panic("TODO: implement New")
}

// Get returns the value and whether it was present, marking it most-recently-used.
func (c *Cache[K, V]) Get(key K) (V, bool) {
	panic("TODO: implement Get")
}

// Put inserts/updates, evicting the least-recently-used entry when full.
func (c *Cache[K, V]) Put(key K, value V) {
	panic("TODO: implement Put")
}

// Len returns the number of stored entries.
func (c *Cache[K, V]) Len() int {
	panic("TODO: implement Len")
}

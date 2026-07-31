// Package rwmutexcache — Exercise 041 (intermediate).
// Goal:   Implement a thread-safe key-value Cache using sync.RWMutex,
//         allowing many concurrent readers or one exclusive writer.
// Drills: sync.RWMutex, concurrent map access, goroutines, race safety.
package rwmutexcache

import "sync"

// Cache is a concurrency-safe string-to-int key-value store.
type Cache struct {
	mu   sync.RWMutex
	data map[string]int
}

// NewCache creates an empty, ready-to-use Cache.
func NewCache() *Cache {
	panic("TODO: implement NewCache")
}

// Set stores value under key, replacing any existing value.
func (c *Cache) Set(key string, value int) {
	panic("TODO: implement Set")
}

// Get returns the value stored under key and whether it was present.
func (c *Cache) Get(key string) (int, bool) {
	panic("TODO: implement Get")
}

// Delete removes key from the cache, if present.
func (c *Cache) Delete(key string) {
	panic("TODO: implement Delete")
}

// Len returns the number of keys currently in the cache.
func (c *Cache) Len() int {
	panic("TODO: implement Len")
}

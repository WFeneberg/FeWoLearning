// Package rwmutexcache — Exercise 041 (reference solution).
package rwmutexcache

import "sync"

// Cache is a concurrency-safe string-to-int key-value store.
type Cache struct {
	mu   sync.RWMutex
	data map[string]int
}

// NewCache creates an empty, ready-to-use Cache.
func NewCache() *Cache {
	return &Cache{data: make(map[string]int)}
}

// Set stores value under key, replacing any existing value.
func (c *Cache) Set(key string, value int) {
	c.mu.Lock()
	defer c.mu.Unlock()
	c.data[key] = value
}

// Get returns the value stored under key and whether it was present.
func (c *Cache) Get(key string) (int, bool) {
	c.mu.RLock()
	defer c.mu.RUnlock()
	v, ok := c.data[key]
	return v, ok
}

// Delete removes key from the cache, if present.
func (c *Cache) Delete(key string) {
	c.mu.Lock()
	defer c.mu.Unlock()
	delete(c.data, key)
}

// Len returns the number of keys currently in the cache.
func (c *Cache) Len() int {
	c.mu.RLock()
	defer c.mu.RUnlock()
	return len(c.data)
}

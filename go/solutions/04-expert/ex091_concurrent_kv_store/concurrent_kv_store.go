// Package concurrentkvstore — Exercise 091 (reference solution).
package concurrentkvstore

import (
	"hash/fnv"
	"sync"
)

// shard is one independently-locked bucket of the store.
type shard[V any] struct {
	mu   sync.RWMutex
	data map[string]V
}

// Store is a generic, concurrency-safe key-value store that shards its
// entries across a fixed number of independent buckets to reduce lock
// contention under concurrent access.
type Store[V any] struct {
	shards []*shard[V]
}

// New creates a Store with the given number of shards. It panics if
// numShards <= 0.
func New[V any](numShards int) *Store[V] {
	if numShards <= 0 {
		panic("numShards must be positive")
	}
	shards := make([]*shard[V], numShards)
	for i := range shards {
		shards[i] = &shard[V]{data: make(map[string]V)}
	}
	return &Store[V]{shards: shards}
}

// hashKey deterministically hashes key using FNV-1a.
func hashKey(key string) uint32 {
	h := fnv.New32a()
	// Hash.Write on the standard fnv implementation never returns an error.
	_, _ = h.Write([]byte(key))
	return h.Sum32()
}

// shardFor returns the shard responsible for key.
func (s *Store[V]) shardFor(key string) *shard[V] {
	idx := int(hashKey(key) % uint32(len(s.shards)))
	return s.shards[idx]
}

// Set stores value under key, creating or overwriting the entry.
func (s *Store[V]) Set(key string, value V) {
	sh := s.shardFor(key)
	sh.mu.Lock()
	sh.data[key] = value
	sh.mu.Unlock()
}

// Get returns the value stored under key and whether it was present.
func (s *Store[V]) Get(key string) (V, bool) {
	sh := s.shardFor(key)
	sh.mu.RLock()
	v, ok := sh.data[key]
	sh.mu.RUnlock()
	return v, ok
}

// Delete removes key from the store. It is a no-op if key is absent.
func (s *Store[V]) Delete(key string) {
	sh := s.shardFor(key)
	sh.mu.Lock()
	delete(sh.data, key)
	sh.mu.Unlock()
}

// Len returns the total number of entries currently stored across all
// shards.
func (s *Store[V]) Len() int {
	total := 0
	for _, sh := range s.shards {
		sh.mu.RLock()
		total += len(sh.data)
		sh.mu.RUnlock()
	}
	return total
}

// NumShards returns the number of shards the store was created with.
func (s *Store[V]) NumShards() int {
	return len(s.shards)
}

// ShardIndex returns the index of the shard that key hashes to. Exposed so
// tests can verify keys are actually distributed across shards.
func (s *Store[V]) ShardIndex(key string) int {
	return int(hashKey(key) % uint32(len(s.shards)))
}

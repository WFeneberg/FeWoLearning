// Package concurrentkvstore — Exercise 091 (expert).
// Goal:   A sharded, mutex-protected in-memory key-value store. Keys are
//         distributed across a fixed number of independent shards (each
//         guarded by its own sync.RWMutex) so unrelated keys can be read and
//         written concurrently without contending on a single global lock.
// Drills: sync.RWMutex, hashing for shard selection, generics, goroutine-safe
//         data structures, designing APIs that must survive `go test -race`.
package concurrentkvstore

// Store is a generic, concurrency-safe key-value store that shards its
// entries across a fixed number of independent buckets to reduce lock
// contention under concurrent access.
type Store[V any] struct {
	// TODO: add fields (shards []*shard[V], numShards int, ...)
}

// New creates a Store with the given number of shards. It panics if
// numShards <= 0.
func New[V any](numShards int) *Store[V] {
	panic("TODO: implement New")
}

// Set stores value under key, creating or overwriting the entry.
func (s *Store[V]) Set(key string, value V) {
	panic("TODO: implement Set")
}

// Get returns the value stored under key and whether it was present.
func (s *Store[V]) Get(key string) (V, bool) {
	panic("TODO: implement Get")
}

// Delete removes key from the store. It is a no-op if key is absent.
func (s *Store[V]) Delete(key string) {
	panic("TODO: implement Delete")
}

// Len returns the total number of entries currently stored across all
// shards.
func (s *Store[V]) Len() int {
	panic("TODO: implement Len")
}

// NumShards returns the number of shards the store was created with.
func (s *Store[V]) NumShards() int {
	panic("TODO: implement NumShards")
}

// ShardIndex returns the index of the shard that key hashes to. Exposed so
// tests can verify keys are actually distributed across shards.
func (s *Store[V]) ShardIndex(key string) int {
	panic("TODO: implement ShardIndex")
}

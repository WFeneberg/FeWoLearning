// Package bufferreusepool — Exercise 088 (advanced).
// Goal:   A SizedBufferPool hands out zeroed []byte buffers from
//         size-bucketed sync.Pool instances, rounding each request up to the
//         next power-of-two "bucket" capacity. A buffer returned via Put is
//         reset (zeroed, len truncated) and stored under the bucket matching
//         its capacity, so a later Get for a size in the same bucket reuses
//         the identical underlying array instead of allocating.
// Drills: sync.Pool, size-class bucketing, bit tricks, slice aliasing.
package bufferreusepool

// SizedBufferPool hands out buffers sized to power-of-two capacity buckets,
// reusing returned buffers whose bucket matches a later request.
type SizedBufferPool struct {
	// TODO: add fields (e.g. a map from bucket capacity to *sync.Pool,
	// plus a mutex if the map is created lazily).
}

// NewSizedBufferPool creates an empty pool.
func NewSizedBufferPool() *SizedBufferPool {
	panic("TODO: implement NewSizedBufferPool")
}

// Get returns a zeroed []byte of length size, backed by an array whose
// capacity is the smallest power of two >= size (minimum 64). It panics if
// size < 0. Buffers previously returned to the pool via Put for the same
// bucket are reused when available.
func (p *SizedBufferPool) Get(size int) []byte {
	panic("TODO: implement Get")
}

// Put clears buf's contents and returns its backing array to the pool
// bucket matching cap(buf), making it available to a future Get call for
// that bucket. Put is a no-op for a nil or zero-capacity buffer.
func (p *SizedBufferPool) Put(buf []byte) {
	panic("TODO: implement Put")
}

// BucketCapacity returns the bucket capacity that Get(size) would use,
// i.e. the smallest power of two >= size, floored at 64. It panics if
// size < 0.
func BucketCapacity(size int) int {
	panic("TODO: implement BucketCapacity")
}

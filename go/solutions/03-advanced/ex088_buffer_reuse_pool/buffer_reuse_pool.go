// Package bufferreusepool — Exercise 088 (reference solution).
package bufferreusepool

import "sync"

// minBucket is the smallest bucket capacity ever handed out.
const minBucket = 64

// SizedBufferPool hands out buffers sized to power-of-two capacity buckets,
// reusing returned buffers whose bucket matches a later request.
type SizedBufferPool struct {
	mu      sync.Mutex
	buckets map[int]*sync.Pool
}

// NewSizedBufferPool creates an empty pool.
func NewSizedBufferPool() *SizedBufferPool {
	return &SizedBufferPool{buckets: make(map[int]*sync.Pool)}
}

// BucketCapacity returns the bucket capacity that Get(size) would use,
// i.e. the smallest power of two >= size, floored at 64. It panics if
// size < 0.
func BucketCapacity(size int) int {
	if size < 0 {
		panic("bufferreusepool: negative size")
	}
	bucketCap := minBucket
	for bucketCap < size {
		bucketCap <<= 1
	}
	return bucketCap
}

// poolFor returns (creating if necessary) the sync.Pool for bucket capacity c.
func (p *SizedBufferPool) poolFor(c int) *sync.Pool {
	p.mu.Lock()
	defer p.mu.Unlock()
	pool, ok := p.buckets[c]
	if !ok {
		bucketCap := c
		pool = &sync.Pool{
			New: func() any {
				return make([]byte, bucketCap)
			},
		}
		p.buckets[c] = pool
	}
	return pool
}

// Get returns a zeroed []byte of length size, backed by an array whose
// capacity is the smallest power of two >= size (minimum 64). It panics if
// size < 0. Buffers previously returned to the pool via Put for the same
// bucket are reused when available.
func (p *SizedBufferPool) Get(size int) []byte {
	bucketCap := BucketCapacity(size)
	pool := p.poolFor(bucketCap)
	buf := pool.Get().([]byte)
	buf = buf[:cap(buf)]
	for i := range buf {
		buf[i] = 0
	}
	return buf[:size]
}

// Put clears buf's contents and returns its backing array to the pool
// bucket matching cap(buf), making it available to a future Get call for
// that bucket. Put is a no-op for a nil or zero-capacity buffer.
func (p *SizedBufferPool) Put(buf []byte) {
	c := cap(buf)
	if c == 0 {
		return
	}
	full := buf[:c]
	for i := range full {
		full[i] = 0
	}
	pool := p.poolFor(c)
	pool.Put(full)
}

// Package syncpoolbuffers — Exercise 076 (reference solution).
package syncpoolbuffers

import (
	"bytes"
	"sync"
)

// BufferPool hands out *bytes.Buffer values for temporary use and reclaims
// them via Put so their backing array can be reused by a later Get.
type BufferPool struct {
	pool sync.Pool
}

// NewBufferPool creates a ready-to-use BufferPool.
func NewBufferPool() *BufferPool {
	return &BufferPool{
		pool: sync.Pool{
			New: func() any {
				return new(bytes.Buffer)
			},
		},
	}
}

// Get returns a *bytes.Buffer that is guaranteed to be empty (Len() == 0),
// either freshly allocated or recycled from a previous Put.
func (p *BufferPool) Get() *bytes.Buffer {
	return p.pool.Get().(*bytes.Buffer)
}

// Put returns buf to the pool for reuse. buf must not be used by the caller
// again after Put. The buffer is reset before being stored so no data from
// this use leaks into a future Get.
func (p *BufferPool) Put(buf *bytes.Buffer) {
	if buf == nil {
		return
	}
	buf.Reset()
	p.pool.Put(buf)
}

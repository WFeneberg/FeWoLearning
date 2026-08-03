// Package syncpoolbuffers — Exercise 076 (advanced).
// Goal:   A BufferPool wrapping sync.Pool that hands out reset *bytes.Buffer
//         values, so callers can reuse the underlying byte array instead of
//         allocating a fresh buffer on every request.
// Drills: sync.Pool, bytes.Buffer, avoiding allocation churn.
package syncpoolbuffers

import (
	"bytes"
	"sync"
)

// BufferPool hands out *bytes.Buffer values for temporary use and reclaims
// them via Put so their backing array can be reused by a later Get.
type BufferPool struct {
	// TODO: wire this pool up in NewBufferPool by giving it a New func that
	// allocates a fresh *bytes.Buffer.
	pool sync.Pool
}

// NewBufferPool creates a ready-to-use BufferPool.
func NewBufferPool() *BufferPool {
	panic("TODO: implement NewBufferPool")
}

// Get returns a *bytes.Buffer that is guaranteed to be empty (Len() == 0),
// either freshly allocated or recycled from a previous Put.
func (p *BufferPool) Get() *bytes.Buffer {
	panic("TODO: implement Get")
}

// Put returns buf to the pool for reuse. buf must not be used by the caller
// again after Put. The buffer is reset before being stored so no data from
// this use leaks into a future Get.
func (p *BufferPool) Put(buf *bytes.Buffer) {
	panic("TODO: implement Put")
}

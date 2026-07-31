// Package connectionpool — Exercise 097 (expert).
// Goal:   Fixed-size Pool of reusable fake Connections. Acquire hands out an
//         idle Connection or lazily creates a new one while the pool has not
//         yet reached its size; once the size is reached Acquire blocks until
//         another goroutine calls Release. The pool must never create more
//         than size distinct Connections.
// Drills: sync.Mutex + sync.Cond, bounded resource pools, goroutine blocking.
package connectionpool

// Connection is a fake, reusable resource managed by a Pool.
type Connection struct {
	ID int
}

// Pool is a fixed-size pool of reusable Connections.
type Pool struct {
	// TODO: add fields (mutex/cond, idle slice, created counter, size)
}

// NewPool creates a Pool that lazily creates up to size Connections.
// It panics if size <= 0.
func NewPool(size int) *Pool {
	panic("TODO: implement NewPool")
}

// Acquire returns an idle Connection, creating a new one if the pool has
// not yet reached its size. If the pool is exhausted (size Connections are
// already checked out), Acquire blocks until a Release makes one available.
func (p *Pool) Acquire() *Connection {
	panic("TODO: implement Acquire")
}

// Release returns c to the pool, making it available for reuse and waking
// one blocked Acquire caller, if any.
func (p *Pool) Release(c *Connection) {
	panic("TODO: implement Release")
}

// Created returns the total number of distinct Connections the Pool has
// ever created. It never exceeds the Pool's size.
func (p *Pool) Created() int {
	panic("TODO: implement Created")
}

// Package connectionpool — Exercise 097 (reference solution).
package connectionpool

import "sync"

// Connection is a fake, reusable resource managed by a Pool.
type Connection struct {
	ID int
}

// Pool is a fixed-size pool of reusable Connections.
type Pool struct {
	mu      sync.Mutex
	cond    *sync.Cond
	size    int
	created int
	idle    []*Connection
}

// NewPool creates a Pool that lazily creates up to size Connections.
// It panics if size <= 0.
func NewPool(size int) *Pool {
	if size <= 0 {
		panic("connectionpool: size must be positive")
	}
	p := &Pool{size: size}
	p.cond = sync.NewCond(&p.mu)
	return p
}

// Acquire returns an idle Connection, creating a new one if the pool has
// not yet reached its size. If the pool is exhausted (size Connections are
// already checked out), Acquire blocks until a Release makes one available.
func (p *Pool) Acquire() *Connection {
	p.mu.Lock()
	defer p.mu.Unlock()

	for {
		if n := len(p.idle); n > 0 {
			c := p.idle[n-1]
			p.idle = p.idle[:n-1]
			return c
		}
		if p.created < p.size {
			p.created++
			return &Connection{ID: p.created}
		}
		// Pool exhausted: wait for a Release to signal us.
		p.cond.Wait()
	}
}

// Release returns c to the pool, making it available for reuse and waking
// one blocked Acquire caller, if any.
func (p *Pool) Release(c *Connection) {
	p.mu.Lock()
	p.idle = append(p.idle, c)
	p.mu.Unlock()
	p.cond.Signal()
}

// Created returns the total number of distinct Connections the Pool has
// ever created. It never exceeds the Pool's size.
func (p *Pool) Created() int {
	p.mu.Lock()
	defer p.mu.Unlock()
	return p.created
}

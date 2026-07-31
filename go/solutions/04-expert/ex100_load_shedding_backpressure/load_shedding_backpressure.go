// Package loadsheddingbackpressure — Exercise 100 (reference solution).
package loadsheddingbackpressure

import (
	"errors"
	"sync"
)

// ErrQueueFull is returned by Submit when the server's request queue is
// saturated and the request has been shed instead of accepted.
var ErrQueueFull = errors.New("load shed: queue full")

// Server is a bounded-queue server. It accepts up to Capacity() concurrent
// requests; once that many requests are accepted and not yet Complete'd,
// further Submit calls fail fast with ErrQueueFull instead of blocking.
type Server struct {
	mu       sync.Mutex
	capacity int
	queued   int
}

// NewServer creates a Server with the given queue capacity. It panics if
// capacity <= 0.
func NewServer(capacity int) *Server {
	if capacity <= 0 {
		panic("loadsheddingbackpressure: capacity must be positive")
	}
	return &Server{capacity: capacity}
}

// Submit attempts to enqueue one unit of work without blocking. It returns
// nil if the queue had room and the request was accepted, or ErrQueueFull
// if the queue was already at capacity and the request was shed.
func (s *Server) Submit() error {
	s.mu.Lock()
	defer s.mu.Unlock()

	if s.queued >= s.capacity {
		return ErrQueueFull
	}
	s.queued++
	return nil
}

// Complete marks one previously accepted request as finished, freeing a
// slot in the queue for a future Submit. It panics if called when no
// requests are currently queued.
func (s *Server) Complete() {
	s.mu.Lock()
	defer s.mu.Unlock()

	if s.queued <= 0 {
		panic("loadsheddingbackpressure: Complete called with empty queue")
	}
	s.queued--
}

// QueueLen returns the current number of accepted, not-yet-completed requests.
func (s *Server) QueueLen() int {
	s.mu.Lock()
	defer s.mu.Unlock()
	return s.queued
}

// Capacity returns the server's configured queue capacity.
func (s *Server) Capacity() int {
	// capacity is set once in NewServer and never mutated, so it is safe
	// to read without holding the mutex.
	return s.capacity
}

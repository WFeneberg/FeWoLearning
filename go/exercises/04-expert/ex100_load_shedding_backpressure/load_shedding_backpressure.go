// Package loadsheddingbackpressure — Exercise 100 (expert).
// Goal:   A Server with a bounded request queue that immediately rejects
//         incoming requests once the queue is full, instead of blocking
//         the caller (load shedding under backpressure).
// Drills: concurrency-safe counters, mutexes, sentinel errors, errors.Is.
package loadsheddingbackpressure

import "errors"

// ErrQueueFull is returned by Submit when the server's request queue is
// saturated and the request has been shed instead of accepted.
var ErrQueueFull = errors.New("load shed: queue full")

// Server is a bounded-queue server. It accepts up to Capacity() concurrent
// requests; once that many requests are accepted and not yet Complete'd,
// further Submit calls fail fast with ErrQueueFull instead of blocking.
type Server struct {
	// TODO: add fields (capacity, synchronization primitive, in-flight count)
}

// NewServer creates a Server with the given queue capacity. It panics if
// capacity <= 0.
func NewServer(capacity int) *Server {
	panic("TODO: implement NewServer")
}

// Submit attempts to enqueue one unit of work without blocking. It returns
// nil if the queue had room and the request was accepted, or ErrQueueFull
// if the queue was already at capacity and the request was shed.
func (s *Server) Submit() error {
	panic("TODO: implement Submit")
}

// Complete marks one previously accepted request as finished, freeing a
// slot in the queue for a future Submit. It panics if called when no
// requests are currently queued.
func (s *Server) Complete() {
	panic("TODO: implement Complete")
}

// QueueLen returns the current number of accepted, not-yet-completed requests.
func (s *Server) QueueLen() int {
	panic("TODO: implement QueueLen")
}

// Capacity returns the server's configured queue capacity.
func (s *Server) Capacity() int {
	panic("TODO: implement Capacity")
}

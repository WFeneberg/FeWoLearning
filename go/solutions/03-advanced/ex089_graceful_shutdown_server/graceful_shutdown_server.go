// Package gracefulshutdownserver — Exercise 089 (reference solution).
package gracefulshutdownserver

import (
	"context"
	"errors"
	"sync"
)

// ErrServerClosed is returned by Serve once the server has begun shutting down.
var ErrServerClosed = errors.New("gracefulshutdownserver: server closed")

// Server runs work items ("requests") in the background while tracking how
// many are currently in flight, so that Shutdown can wait for them to drain.
type Server struct {
	mu     sync.Mutex
	closed bool
	wg     sync.WaitGroup
}

// New returns a Server ready to accept work.
func New() *Server {
	return &Server{}
}

// Serve runs work in a new goroutine, tracking it as in-flight.
// If the server is shutting down (or has shut down), it does not start
// work and returns ErrServerClosed instead.
func (s *Server) Serve(work func()) error {
	s.mu.Lock()
	if s.closed {
		s.mu.Unlock()
		return ErrServerClosed
	}
	s.wg.Add(1)
	s.mu.Unlock()

	go func() {
		defer s.wg.Done()
		work()
	}()
	return nil
}

// Shutdown marks the server as closed so that no further calls to Serve are
// accepted, then blocks until every in-flight call to work started by Serve
// has finished. If ctx is done before that happens, Shutdown returns
// ctx.Err() without waiting any further.
func (s *Server) Shutdown(ctx context.Context) error {
	s.mu.Lock()
	s.closed = true
	s.mu.Unlock()

	drained := make(chan struct{})
	go func() {
		s.wg.Wait()
		close(drained)
	}()

	select {
	case <-drained:
		return nil
	case <-ctx.Done():
		return ctx.Err()
	}
}

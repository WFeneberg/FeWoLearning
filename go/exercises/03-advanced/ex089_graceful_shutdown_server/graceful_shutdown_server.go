// Package gracefulshutdownserver — Exercise 089 (advanced).
// Goal:   A Server that tracks in-flight work and shuts down gracefully:
//         Shutdown stops accepting new work immediately but waits for
//         work already in flight to finish (or for the context to expire).
// Drills: sync.WaitGroup, sync.Mutex, context cancellation, select.
package gracefulshutdownserver

import (
	"context"
	"errors"
)

// ErrServerClosed is returned by Serve once the server has begun shutting down.
var ErrServerClosed = errors.New("gracefulshutdownserver: server closed")

// Server runs work items ("requests") in the background while tracking how
// many are currently in flight, so that Shutdown can wait for them to drain.
type Server struct {
	// TODO: add fields (mutex, closed flag, WaitGroup)
}

// New returns a Server ready to accept work.
func New() *Server {
	panic("TODO: implement New")
}

// Serve runs work in a new goroutine, tracking it as in-flight.
// If the server is shutting down (or has shut down), it does not start
// work and returns ErrServerClosed instead.
func (s *Server) Serve(work func()) error {
	panic("TODO: implement Serve")
}

// Shutdown marks the server as closed so that no further calls to Serve are
// accepted, then blocks until every in-flight call to work started by Serve
// has finished. If ctx is done before that happens, Shutdown returns
// ctx.Err() without waiting any further.
func (s *Server) Shutdown(ctx context.Context) error {
	panic("TODO: implement Shutdown")
}

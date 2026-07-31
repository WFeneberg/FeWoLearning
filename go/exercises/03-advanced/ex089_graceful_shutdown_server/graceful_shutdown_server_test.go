package gracefulshutdownserver

import (
	"context"
	"errors"
	"runtime"
	"testing"
)

// TestServeRunsWork checks the happy path: accepted work actually executes.
func TestServeRunsWork(t *testing.T) {
	s := New()
	done := make(chan struct{})
	if err := s.Serve(func() { close(done) }); err != nil {
		t.Fatalf("Serve() error = %v, want nil", err)
	}
	<-done // must not deadlock: work really runs
}

// TestShutdownOnIdleServerReturnsNil checks Shutdown with nothing in flight.
func TestShutdownOnIdleServerReturnsNil(t *testing.T) {
	s := New()
	if err := s.Shutdown(context.Background()); err != nil {
		t.Fatalf("Shutdown() error = %v, want nil", err)
	}
}

// TestServeAfterShutdownRejected checks that a fully shut-down server rejects
// all further work with ErrServerClosed.
func TestServeAfterShutdownRejected(t *testing.T) {
	s := New()
	if err := s.Shutdown(context.Background()); err != nil {
		t.Fatalf("Shutdown() error = %v, want nil", err)
	}
	if err := s.Serve(func() {}); !errors.Is(err, ErrServerClosed) {
		t.Fatalf("Serve() after Shutdown = %v, want ErrServerClosed", err)
	}
}

// TestShutdownWaitsForInFlightAndRejectsNew is the core scenario: while a
// handler is in flight, Shutdown must (a) immediately stop accepting new
// work, and (b) still wait for the in-flight handler to finish, returning
// nil only once it has.
func TestShutdownWaitsForInFlightAndRejectsNew(t *testing.T) {
	s := New()

	started := make(chan struct{})
	release := make(chan struct{})
	completed := make(chan struct{})

	if err := s.Serve(func() {
		close(started)
		<-release
		close(completed)
	}); err != nil {
		t.Fatalf("Serve() error = %v, want nil", err)
	}
	<-started // the handler is now in flight

	shutdownErr := make(chan error, 1)
	go func() { shutdownErr <- s.Shutdown(context.Background()) }()

	// Spin (no sleeping, no wall-clock) until Shutdown has flipped the
	// server closed, proving new work is rejected immediately, i.e. before
	// the in-flight handler is allowed to finish.
	var rejectErr error
	accepted := 0
	for i := 0; i < 1_000_000; i++ {
		err := s.Serve(func() {})
		if err != nil {
			rejectErr = err
			break
		}
		accepted++
		runtime.Gosched()
	}
	if !errors.Is(rejectErr, ErrServerClosed) {
		t.Fatalf("Serve() during shutdown = %v, want ErrServerClosed (accepted %d before rejection)", rejectErr, accepted)
	}

	// The in-flight handler must still be blocked: Shutdown does not cut it
	// short, it only stops accepting new work.
	select {
	case <-completed:
		t.Fatal("in-flight handler completed before it was released")
	default:
	}

	// Also confirm Shutdown itself hasn't returned yet (it must wait).
	select {
	case err := <-shutdownErr:
		t.Fatalf("Shutdown() returned early with err = %v, want it to keep waiting", err)
	default:
	}

	close(release) // let the in-flight handler finish

	<-completed // the in-flight handler ran to completion successfully

	if err := <-shutdownErr; err != nil {
		t.Fatalf("Shutdown() error = %v, want nil", err)
	}
}

// TestShutdownContextCanceledReturnsCtxErr checks that Shutdown gives up and
// reports the context error if the context is done before in-flight work
// finishes, without waiting for it.
func TestShutdownContextCanceledReturnsCtxErr(t *testing.T) {
	s := New()

	started := make(chan struct{})
	release := make(chan struct{})
	if err := s.Serve(func() {
		close(started)
		<-release
	}); err != nil {
		t.Fatalf("Serve() error = %v, want nil", err)
	}
	<-started

	ctx, cancel := context.WithCancel(context.Background())
	cancel() // already canceled: Shutdown must not block waiting on the handler

	if err := s.Shutdown(ctx); !errors.Is(err, context.Canceled) {
		t.Fatalf("Shutdown() error = %v, want context.Canceled", err)
	}

	close(release) // clean up the still in-flight goroutine
}

// Package jobschedulercron — Exercise 095 (expert).
// Goal:   A Scheduler that runs a registered job repeatedly at a fixed
//         interval until Stop is called. Production code drives the
//         schedule from a real time.Ticker, but the Scheduler accepts any
//         <-chan time.Time as its tick source so tests can feed ticks by
//         hand and stay fully deterministic (no sleeping, no wall-clock
//         waits, no flaky timing assertions).
// Drills: goroutines, channels, select, sync.Mutex, sync.Once, graceful
//         shutdown, designing concurrent APIs for testability.
package jobschedulercron

import (
	"sync"
	"time"
)

// Scheduler runs a job every time it receives a tick, until Stop is called.
type Scheduler struct {
	job       func()
	ticks     <-chan time.Time
	stopTick  func() // stops the underlying real ticker, if any; may be nil

	stopCh   chan struct{}
	doneCh   chan struct{}
	stopOnce sync.Once

	mu       sync.Mutex
	started  bool
	runCount int
}

// New creates a Scheduler that invokes job every interval, using a real
// time.Ticker as its tick source. It panics if interval <= 0 or job is nil.
func New(interval time.Duration, job func()) *Scheduler {
	panic("TODO: implement New")
}

// NewWithTicks creates a Scheduler driven by an externally supplied tick
// channel instead of a real timer. This is the hook tests use to control
// the schedule deterministically. It panics if job is nil.
func NewWithTicks(ticks <-chan time.Time, job func()) *Scheduler {
	panic("TODO: implement NewWithTicks")
}

// Start begins running the scheduler in a background goroutine. It panics
// if the scheduler was already started.
func (s *Scheduler) Start() {
	panic("TODO: implement Start")
}

// Stop signals the background goroutine to exit and blocks until it has
// fully stopped. Calling Stop more than once, or before Start, is a no-op.
func (s *Scheduler) Stop() {
	panic("TODO: implement Stop")
}

// RunCount returns how many times the job has completed so far. Safe to
// call concurrently with Start/Stop.
func (s *Scheduler) RunCount() int {
	panic("TODO: implement RunCount")
}

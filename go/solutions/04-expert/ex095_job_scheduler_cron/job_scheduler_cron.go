// Package jobschedulercron — Exercise 095 (reference solution).
package jobschedulercron

import (
	"sync"
	"time"
)

// Scheduler runs a job every time it receives a tick, until Stop is called.
type Scheduler struct {
	job      func()
	ticks    <-chan time.Time
	stopTick func() // stops the underlying real ticker, if any; may be nil

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
	if interval <= 0 {
		panic("jobschedulercron: interval must be positive")
	}
	ticker := time.NewTicker(interval)
	s := NewWithTicks(ticker.C, job)
	s.stopTick = ticker.Stop
	return s
}

// NewWithTicks creates a Scheduler driven by an externally supplied tick
// channel instead of a real timer. This is the hook tests use to control
// the schedule deterministically. It panics if job is nil.
func NewWithTicks(ticks <-chan time.Time, job func()) *Scheduler {
	if job == nil {
		panic("jobschedulercron: job must not be nil")
	}
	return &Scheduler{
		job:    job,
		ticks:  ticks,
		stopCh: make(chan struct{}),
		doneCh: make(chan struct{}),
	}
}

// Start begins running the scheduler in a background goroutine. It panics
// if the scheduler was already started.
func (s *Scheduler) Start() {
	s.mu.Lock()
	if s.started {
		s.mu.Unlock()
		panic("jobschedulercron: scheduler already started")
	}
	s.started = true
	s.mu.Unlock()

	go s.run()
}

func (s *Scheduler) run() {
	defer close(s.doneCh)
	for {
		select {
		case <-s.ticks:
			s.mu.Lock()
			s.runCount++
			s.mu.Unlock()
			s.job()
		case <-s.stopCh:
			return
		}
	}
}

// Stop signals the background goroutine to exit and blocks until it has
// fully stopped. Calling Stop more than once, or before Start, is a no-op.
func (s *Scheduler) Stop() {
	s.mu.Lock()
	started := s.started
	s.mu.Unlock()
	if !started {
		return
	}

	s.stopOnce.Do(func() {
		close(s.stopCh)
		<-s.doneCh
		if s.stopTick != nil {
			s.stopTick()
		}
	})
}

// RunCount returns how many times the job has completed so far. Safe to
// call concurrently with Start/Stop.
func (s *Scheduler) RunCount() int {
	s.mu.Lock()
	defer s.mu.Unlock()
	return s.runCount
}

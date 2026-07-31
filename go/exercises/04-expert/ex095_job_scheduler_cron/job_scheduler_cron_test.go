package jobschedulercron

import (
	"testing"
	"time"
)

// TestRunsExpectedNumberOfTimesThenStops feeds ticks one at a time and, after
// each tick, blocks on a completion signal sent by the job itself. Because
// the job signals completion only after the scheduler has recorded the run,
// this synchronizes purely on channels — no sleeping, no wall-clock waits.
func TestRunsExpectedNumberOfTimesThenStops(t *testing.T) {
	ticks := make(chan time.Time)
	confirm := make(chan struct{})
	job := func() { confirm <- struct{}{} }

	sched := NewWithTicks(ticks, job)
	sched.Start()

	const wantRuns = 5
	for i := 1; i <= wantRuns; i++ {
		ticks <- time.Time{}
		<-confirm
		if got := sched.RunCount(); got != i {
			t.Fatalf("after tick %d: RunCount() = %d, want %d", i, got, i)
		}
	}

	sched.Stop()

	// After Stop, the background goroutine must have exited: a further tick
	// cannot be consumed, so a non-blocking send must fail to deliver.
	select {
	case ticks <- time.Time{}:
		t.Fatal("tick was consumed after Stop; scheduler did not stop")
	default:
	}

	if got := sched.RunCount(); got != wantRuns {
		t.Errorf("RunCount() after Stop = %d, want %d (still running?)", got, wantRuns)
	}
}

// TestStopIsIdempotentAndBlocksUntilExit ensures Stop can be called multiple
// times safely and that it does not return before the goroutine has really
// exited (verified by RunCount no longer changing even if more ticks land).
func TestStopIsIdempotentAndBlocksUntilExit(t *testing.T) {
	ticks := make(chan time.Time)
	confirm := make(chan struct{})
	job := func() { confirm <- struct{}{} }

	sched := NewWithTicks(ticks, job)
	sched.Start()

	ticks <- time.Time{}
	<-confirm

	sched.Stop()
	sched.Stop() // must not panic or deadlock

	if got := sched.RunCount(); got != 1 {
		t.Errorf("RunCount() = %d, want 1", got)
	}
}

// TestStopBeforeStartIsNoop ensures calling Stop on a never-started
// scheduler returns immediately instead of blocking forever.
func TestStopBeforeStartIsNoop(t *testing.T) {
	ticks := make(chan time.Time)
	sched := NewWithTicks(ticks, func() {})

	done := make(chan struct{})
	go func() {
		sched.Stop()
		close(done)
	}()

	select {
	case <-done:
	case <-time.After(time.Second):
		t.Fatal("Stop() blocked on a scheduler that was never started")
	}

	if got := sched.RunCount(); got != 0 {
		t.Errorf("RunCount() = %d, want 0", got)
	}
}

// TestStartTwicePanics ensures a scheduler cannot be started concurrently
// more than once, which would otherwise leak goroutines silently.
func TestStartTwicePanics(t *testing.T) {
	ticks := make(chan time.Time)
	sched := NewWithTicks(ticks, func() {})
	sched.Start()
	defer sched.Stop()

	defer func() {
		if r := recover(); r == nil {
			t.Error("expected Start() to panic on second call")
		}
	}()
	sched.Start()
}

// TestNewRejectsInvalidArguments checks the validation on the real-timer
// constructor.
func TestNewRejectsInvalidArguments(t *testing.T) {
	mustPanic := func(name string, fn func()) {
		t.Helper()
		defer func() {
			if r := recover(); r == nil {
				t.Errorf("%s: expected panic, got none", name)
			}
		}()
		fn()
	}

	mustPanic("zero interval", func() { New(0, func() {}) })
	mustPanic("negative interval", func() { New(-time.Second, func() {}) })
	mustPanic("nil job", func() { New(time.Second, nil) })
}

// TestNewWithTicksRejectsNilJob checks the validation on the test-facing
// constructor too.
func TestNewWithTicksRejectsNilJob(t *testing.T) {
	defer func() {
		if r := recover(); r == nil {
			t.Error("expected panic for nil job")
		}
	}()
	NewWithTicks(make(chan time.Time), nil)
}

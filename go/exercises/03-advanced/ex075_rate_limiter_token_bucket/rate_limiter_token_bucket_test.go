package ratelimitertokenbucket

import (
	"testing"
	"time"
)

// fakeClock is a manually-advanceable clock, so the rate limiter's timing
// logic can be tested deterministically without real sleeps or wall-clock
// reads.
type fakeClock struct {
	t time.Time
}

func (f *fakeClock) now() time.Time { return f.t }

func (f *fakeClock) advance(d time.Duration) { f.t = f.t.Add(d) }

func TestAllowThrottlesBurstToCapacity(t *testing.T) {
	clock := &fakeClock{t: time.Unix(0, 0)}
	// Capacity 3, refill 1 token/sec: a burst of 5 immediate requests
	// (measured within the same instant, i.e. zero elapsed time) must
	// only let the first 3 through.
	b := NewWithClock(3, 1.0, clock.now)

	results := make([]bool, 5)
	for i := range results {
		results[i] = b.Allow()
	}

	wantGranted := 3
	got := 0
	for i, ok := range results {
		if ok {
			got++
		}
		if i < wantGranted && !ok {
			t.Errorf("request %d: got denied, want granted (burst within capacity)", i)
		}
		if i >= wantGranted && ok {
			t.Errorf("request %d: got granted, want denied (burst exceeds capacity)", i)
		}
	}
	if got != wantGranted {
		t.Fatalf("granted = %d, want %d", got, wantGranted)
	}
}

func TestAllowRefillsOverMeasuredWindow(t *testing.T) {
	clock := &fakeClock{t: time.Unix(0, 0)}
	// Capacity 2, refill 1 token/sec: drain the bucket, then advance the
	// clock by a measured 2-second window and confirm exactly 2 more
	// tokens (capped at capacity) become available.
	b := NewWithClock(2, 1.0, clock.now)

	if !b.Allow() || !b.Allow() {
		t.Fatal("expected initial burst of 2 to be granted")
	}
	if b.Allow() {
		t.Fatal("expected bucket to be empty after initial burst")
	}

	clock.advance(2 * time.Second) // window elapses: +2 tokens, capped at 2

	if !b.Allow() {
		t.Error("expected 1st request after refill window to be granted")
	}
	if !b.Allow() {
		t.Error("expected 2nd request after refill window to be granted")
	}
	if b.Allow() {
		t.Error("expected 3rd request after refill window to be denied (capacity cap)")
	}
}

func TestAllowPartialRefillDoesNotGrantExtraToken(t *testing.T) {
	clock := &fakeClock{t: time.Unix(0, 0)}
	// Capacity 1, refill 1 token/sec: after draining, advancing by only
	// 500ms must not be enough to refill a whole token.
	b := NewWithClock(1, 1.0, clock.now)

	if !b.Allow() {
		t.Fatal("expected initial request to be granted")
	}
	if b.Allow() {
		t.Fatal("expected bucket to be empty")
	}

	clock.advance(500 * time.Millisecond)
	if b.Allow() {
		t.Error("expected request to be denied after only a partial refill")
	}

	clock.advance(500 * time.Millisecond) // now a full second has elapsed
	if !b.Allow() {
		t.Error("expected request to be granted once a full token has refilled")
	}
}

func TestNewPanicsOnInvalidArgs(t *testing.T) {
	mustPanic := func(name string, fn func()) {
		defer func() {
			if recover() == nil {
				t.Errorf("%s: expected panic", name)
			}
		}()
		fn()
	}
	mustPanic("zero capacity", func() { New(0, 1.0) })
	mustPanic("negative capacity", func() { New(-1, 1.0) })
	mustPanic("zero refill rate", func() { New(1, 0) })
	mustPanic("negative refill rate", func() { New(1, -1.0) })
}

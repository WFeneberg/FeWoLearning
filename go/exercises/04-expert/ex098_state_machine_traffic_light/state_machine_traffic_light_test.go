package statemachinetrafficlight

import (
	"errors"
	"testing"
)

func TestNextCyclesThroughStates(t *testing.T) {
	tl := New()

	if got := tl.Current(); got != Red {
		t.Fatalf("initial Current() = %v, want %v", got, Red)
	}

	wantSequence := []State{Green, Yellow, Red, Green, Yellow, Red}
	for i, want := range wantSequence {
		if got := tl.Next(); got != want {
			t.Fatalf("step %d: Next() = %v, want %v", i, got, want)
		}
		if got := tl.Current(); got != want {
			t.Fatalf("step %d: Current() = %v, want %v", i, got, want)
		}
	}
}

func TestTransitionAcceptsLegalSuccessor(t *testing.T) {
	tl := New()

	if err := tl.Transition(Green); err != nil {
		t.Fatalf("Transition(Green) from Red: unexpected error %v", err)
	}
	if got := tl.Current(); got != Green {
		t.Fatalf("Current() = %v, want %v", got, Green)
	}

	if err := tl.Transition(Yellow); err != nil {
		t.Fatalf("Transition(Yellow) from Green: unexpected error %v", err)
	}
	if got := tl.Current(); got != Yellow {
		t.Fatalf("Current() = %v, want %v", got, Yellow)
	}

	if err := tl.Transition(Red); err != nil {
		t.Fatalf("Transition(Red) from Yellow: unexpected error %v", err)
	}
	if got := tl.Current(); got != Red {
		t.Fatalf("Current() = %v, want %v", got, Red)
	}
}

func TestTransitionRejectsArbitraryJump(t *testing.T) {
	cases := []struct {
		name string
		from State
		to   State
	}{
		{"red-to-yellow", Red, Yellow},
		{"green-to-red", Green, Red},
		{"yellow-to-green", Yellow, Green},
		{"red-to-red", Red, Red},
	}

	for _, tc := range cases {
		t.Run(tc.name, func(t *testing.T) {
			tl := New()
			// Drive the light to tc.from via legal Next() calls.
			for tl.Current() != tc.from {
				tl.Next()
			}

			err := tl.Transition(tc.to)
			if err == nil {
				t.Fatalf("Transition(%v) from %v: expected error, got nil", tc.to, tc.from)
			}
			if !errors.Is(err, ErrInvalidTransition) {
				t.Errorf("Transition(%v) from %v: error = %v, want wrapping ErrInvalidTransition", tc.to, tc.from, err)
			}
			// State must be unchanged after a rejected transition.
			if got := tl.Current(); got != tc.from {
				t.Errorf("Current() after rejected Transition = %v, want unchanged %v", got, tc.from)
			}
		})
	}
}

func TestStateString(t *testing.T) {
	cases := map[State]string{
		Red:    "Red",
		Green:  "Green",
		Yellow: "Yellow",
	}
	for state, want := range cases {
		if got := state.String(); got != want {
			t.Errorf("State(%d).String() = %q, want %q", int(state), got, want)
		}
	}
}

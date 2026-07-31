// Package statemachinetrafficlight — Exercise 098 (reference solution).
package statemachinetrafficlight

import (
	"errors"
	"fmt"
)

// State is one of the traffic light colors.
type State int

const (
	Red State = iota
	Green
	Yellow
)

// String implements fmt.Stringer for State.
func (s State) String() string {
	switch s {
	case Red:
		return "Red"
	case Green:
		return "Green"
	case Yellow:
		return "Yellow"
	default:
		return fmt.Sprintf("State(%d)", int(s))
	}
}

// ErrInvalidTransition is returned by Transition when the requested target
// state is not the legal successor of the current state.
var ErrInvalidTransition = errors.New("invalid transition")

// successor maps each state to the only state it may legally move to.
func successor(s State) State {
	switch s {
	case Red:
		return Green
	case Green:
		return Yellow
	case Yellow:
		return Red
	default:
		// Unknown states have no defined successor; stay put so callers
		// relying on successor(s) == s can detect the anomaly.
		return s
	}
}

// TrafficLight is a small state machine starting at Red.
type TrafficLight struct {
	current State
}

// New returns a TrafficLight initialized to Red.
func New() *TrafficLight {
	return &TrafficLight{current: Red}
}

// Current returns the current state.
func (t *TrafficLight) Current() State {
	return t.current
}

// Next advances the light to its legal successor (Red->Green->Yellow->Red)
// and returns the new state.
func (t *TrafficLight) Next() State {
	t.current = successor(t.current)
	return t.current
}

// Transition attempts to move the light to target. It succeeds only if
// target is the legal successor of the current state; otherwise it returns
// an error wrapping ErrInvalidTransition and leaves the state unchanged.
func (t *TrafficLight) Transition(target State) error {
	want := successor(t.current)
	if target != want {
		return fmt.Errorf("%w: cannot go from %s to %s (expected %s)", ErrInvalidTransition, t.current, target, want)
	}
	t.current = target
	return nil
}

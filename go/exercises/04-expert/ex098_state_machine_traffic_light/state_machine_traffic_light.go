// Package statemachinetrafficlight — Exercise 098 (expert).
// Goal:   A TrafficLight state machine that cycles Red->Green->Yellow->Red via
//         Next(), and rejects arbitrary jumps (e.g. Red->Yellow) via a
//         Transition(target State) error method.
// Drills: finite state machines, sentinel errors with errors.Is, Stringer.
package statemachinetrafficlight

import "errors"

// State is one of the traffic light colors.
type State int

const (
	Red State = iota
	Green
	Yellow
)

// String implements fmt.Stringer for State.
func (s State) String() string {
	panic("TODO: implement String")
}

// ErrInvalidTransition is returned by Transition when the requested target
// state is not the legal successor of the current state.
var ErrInvalidTransition = errors.New("invalid transition")

// TrafficLight is a small state machine starting at Red.
type TrafficLight struct {
	// TODO: add fields
}

// New returns a TrafficLight initialized to Red.
func New() *TrafficLight {
	panic("TODO: implement New")
}

// Current returns the current state.
func (t *TrafficLight) Current() State {
	panic("TODO: implement Current")
}

// Next advances the light to its legal successor (Red->Green->Yellow->Red)
// and returns the new state.
func (t *TrafficLight) Next() State {
	panic("TODO: implement Next")
}

// Transition attempts to move the light to target. It succeeds only if
// target is the legal successor of the current state; otherwise it returns
// an error wrapping ErrInvalidTransition and leaves the state unchanged.
func (t *TrafficLight) Transition(target State) error {
	panic("TODO: implement Transition")
}

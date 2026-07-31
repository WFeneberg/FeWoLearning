// Package jsoncustommarshaler — Exercise 053 (intermediate).
// Goal:   Implement a custom json.Marshaler on a Duration-like type so it
//         serializes as a human-readable string (e.g. "1h30m0s") instead of
//         the default numeric encoding, then use it inside a larger struct.
// Drills: json.Marshaler interface, encoding/json, time.Duration formatting.
package jsoncustommarshaler

import "time"

// Span wraps a time.Duration and customizes its JSON representation.
type Span struct {
	D time.Duration
}

// MarshalJSON encodes Span as a JSON string using the Go duration format
// (e.g. "1h30m0s"), rather than the underlying integer nanoseconds.
func (s Span) MarshalJSON() ([]byte, error) {
	panic("TODO: implement Span.MarshalJSON")
}

// Task represents a named job with an estimated duration.
type Task struct {
	Name string `json:"name"`
	ETA  Span   `json:"eta"`
}

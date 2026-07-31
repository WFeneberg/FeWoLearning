// Package jsoncustommarshaler — Exercise 053 (reference solution).
package jsoncustommarshaler

import (
	"encoding/json"
	"time"
)

// Span wraps a time.Duration and customizes its JSON representation.
type Span struct {
	D time.Duration
}

// MarshalJSON encodes Span as a JSON string using the Go duration format
// (e.g. "1h30m0s"), rather than the underlying integer nanoseconds.
func (s Span) MarshalJSON() ([]byte, error) {
	return json.Marshal(s.D.String())
}

// Task represents a named job with an estimated duration.
type Task struct {
	Name string `json:"name"`
	ETA  Span   `json:"eta"`
}

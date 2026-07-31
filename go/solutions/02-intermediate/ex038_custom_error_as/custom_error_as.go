// Package customerroras — Exercise 038 (reference solution).
package customerroras

import "fmt"

// NotFoundError indicates that a resource of a given Kind could not be
// located by its ID.
type NotFoundError struct {
	Kind string
	ID   string
}

// Error implements the error interface.
func (e *NotFoundError) Error() string {
	return fmt.Sprintf("%s with id %q not found", e.Kind, e.ID)
}

// Lookup simulates looking up a resource of the given kind by id. It
// returns a *NotFoundError (wrapped, not raw) when the id is not "42".
func Lookup(kind, id string) error {
	if id == "42" {
		return nil
	}
	return fmt.Errorf("lookup failed: %w", &NotFoundError{Kind: kind, ID: id})
}

// FetchResource wraps Lookup in an additional layer of context, simulating
// a higher-level caller that adds its own error wrapping.
func FetchResource(kind, id string) error {
	if err := Lookup(kind, id); err != nil {
		return fmt.Errorf("fetch resource %s/%s: %w", kind, id, err)
	}
	return nil
}

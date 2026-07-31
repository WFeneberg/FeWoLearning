// Package customerroras — Exercise 038 (intermediate).
// Goal:   Define a custom NotFoundError type and return it wrapped inside
//         another error, then use errors.As to extract the concrete type
//         and its fields after it has passed through a wrapping layer.
// Drills: custom error types, error wrapping (%w), errors.As.
package customerroras

// NotFoundError indicates that a resource of a given Kind could not be
// located by its ID.
type NotFoundError struct {
	Kind string
	ID   string
}

// Error implements the error interface.
func (e *NotFoundError) Error() string {
	panic("TODO: implement NotFoundError.Error")
}

// Lookup simulates looking up a resource of the given kind by id. It
// returns a *NotFoundError (wrapped, not raw) when the id is not "42".
func Lookup(kind, id string) error {
	panic("TODO: implement Lookup")
}

// FetchResource wraps Lookup in an additional layer of context, simulating
// a higher-level caller that adds its own error wrapping.
func FetchResource(kind, id string) error {
	panic("TODO: implement FetchResource")
}

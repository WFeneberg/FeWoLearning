// Package errortreemultierror — Exercise 082 (advanced).
// Goal:   A MultiError aggregates several errors (possibly other *MultiError
//         nodes) into a tree and implements Unwrap() []error, so that
//         errors.Is/errors.As can locate a sentinel buried anywhere inside.
// Drills: custom error trees, Unwrap() []error, errors.Is traversal.
package errortreemultierror

// MultiError aggregates zero or more errors. Any element may itself be a
// *MultiError, forming a tree of errors.
type MultiError struct {
	Errs []error
}

// Append filters out nil errors and returns a *MultiError holding the rest.
// It returns nil if no non-nil errors were given.
func Append(errs ...error) *MultiError {
	panic("TODO: implement Append")
}

// Error returns the messages of all aggregated errors joined by "; ".
func (m *MultiError) Error() string {
	panic("TODO: implement Error")
}

// Unwrap exposes the aggregated errors so errors.Is/errors.As can traverse
// into every branch of the tree, including nested *MultiError nodes.
func (m *MultiError) Unwrap() []error {
	panic("TODO: implement Unwrap")
}

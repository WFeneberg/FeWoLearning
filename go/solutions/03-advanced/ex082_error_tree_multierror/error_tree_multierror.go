// Package errortreemultierror — Exercise 082 (reference solution).
package errortreemultierror

import "strings"

// MultiError aggregates zero or more errors. Any element may itself be a
// *MultiError, forming a tree of errors.
type MultiError struct {
	Errs []error
}

// Append filters out nil errors and returns a *MultiError holding the rest.
// It returns nil if no non-nil errors were given.
func Append(errs ...error) *MultiError {
	kept := make([]error, 0, len(errs))
	for _, err := range errs {
		if err != nil {
			kept = append(kept, err)
		}
	}
	if len(kept) == 0 {
		return nil
	}
	return &MultiError{Errs: kept}
}

// Error returns the messages of all aggregated errors joined by "; ".
func (m *MultiError) Error() string {
	msgs := make([]string, len(m.Errs))
	for i, err := range m.Errs {
		msgs[i] = err.Error()
	}
	return strings.Join(msgs, "; ")
}

// Unwrap exposes the aggregated errors so errors.Is/errors.As can traverse
// into every branch of the tree, including nested *MultiError nodes.
func (m *MultiError) Unwrap() []error {
	return m.Errs
}

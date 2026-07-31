// Package errorwrappingchain — Exercise 037 (intermediate).
// Goal:   Build a three-layer call chain (Layer1 -> Layer2 -> Layer3) where the
//         deepest layer returns a sentinel error, and each outer layer wraps
//         the error from the layer it calls using fmt.Errorf("%w", err) so the
//         original sentinel remains discoverable via errors.Is after unwrapping.
// Drills: error wrapping, %w verb, errors.Is, sentinel errors, error chains.
package errorwrappingchain

import "errors"

// ErrSentinel is the root cause error that should remain discoverable
// through errors.Is even after being wrapped multiple times.
var ErrSentinel = errors.New("sentinel: something went wrong")

// Layer3 is the innermost function. It returns ErrSentinel directly.
func Layer3() error {
	panic("TODO: implement Layer3")
}

// Layer2 calls Layer3 and, if it errors, wraps the error with additional
// context using fmt.Errorf("%w", err).
func Layer2() error {
	panic("TODO: implement Layer2")
}

// Layer1 calls Layer2 and, if it errors, wraps the error with additional
// context using fmt.Errorf("%w", err).
func Layer1() error {
	panic("TODO: implement Layer1")
}

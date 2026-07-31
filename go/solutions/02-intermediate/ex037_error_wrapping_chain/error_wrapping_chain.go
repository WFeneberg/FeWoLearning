// Package errorwrappingchain — Exercise 037 (reference solution).
package errorwrappingchain

import (
	"errors"
	"fmt"
)

// ErrSentinel is the root cause error that should remain discoverable
// through errors.Is even after being wrapped multiple times.
var ErrSentinel = errors.New("sentinel: something went wrong")

// Layer3 is the innermost function. It returns ErrSentinel directly.
func Layer3() error {
	return ErrSentinel
}

// Layer2 calls Layer3 and, if it errors, wraps the error with additional
// context using fmt.Errorf("%w", err).
func Layer2() error {
	if err := Layer3(); err != nil {
		return fmt.Errorf("layer2: %w", err)
	}
	return nil
}

// Layer1 calls Layer2 and, if it errors, wraps the error with additional
// context using fmt.Errorf("%w", err).
func Layer1() error {
	if err := Layer2(); err != nil {
		return fmt.Errorf("layer1: %w", err)
	}
	return nil
}

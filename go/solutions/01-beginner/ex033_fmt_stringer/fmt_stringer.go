// Package fmtstringer — Exercise 033 (reference solution).
package fmtstringer

import "fmt"

// Money represents a monetary amount stored in cents.
type Money struct {
	Cents int
}

// String implements fmt.Stringer, formatting the amount as dollars.
func (m Money) String() string {
	dollars := m.Cents / 100
	remainder := m.Cents % 100
	if remainder < 0 {
		remainder = -remainder
	}
	return fmt.Sprintf("$%d.%02d", dollars, remainder)
}

var _ fmt.Stringer = Money{}

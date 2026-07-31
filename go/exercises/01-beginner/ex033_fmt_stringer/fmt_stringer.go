// Package fmtstringer — Exercise 033 (beginner).
// Goal:   Implement the fmt.Stringer interface on a Money type that stores
//         an amount in cents, so it formats itself as dollars (e.g. "$12.34").
// Drills: interfaces, fmt.Stringer, method receivers.
package fmtstringer

import "fmt"

// Money represents a monetary amount stored in cents.
type Money struct {
	Cents int
}

// String implements fmt.Stringer, formatting the amount as dollars,
// e.g. Money{Cents: 1234}.String() == "$12.34".
func (m Money) String() string {
	panic("TODO: implement Money.String")
}

// ensure Money satisfies fmt.Stringer at compile time.
var _ fmt.Stringer = Money{}

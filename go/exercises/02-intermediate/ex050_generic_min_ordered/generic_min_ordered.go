// Package genericminordered — Exercise 050 (intermediate).
// Goal:   Implement a generic Min function that works for any ordered type.
// Drills: generics, type parameters, constraints.Ordered.
package genericminordered

// Ordered is the set of types that support the <, <=, >, >= operators.
// It mirrors the standard library's constraints.Ordered (golang.org/x/exp/constraints),
// defined locally here to avoid an external module dependency.
type Ordered interface {
	~int | ~int8 | ~int16 | ~int32 | ~int64 |
		~uint | ~uint8 | ~uint16 | ~uint32 | ~uint64 | ~uintptr |
		~float32 | ~float64 |
		~string
}

// Min returns the smallest value among vals.
// It panics if vals is empty.
func Min[T Ordered](vals ...T) T {
	panic("TODO: implement Min")
}

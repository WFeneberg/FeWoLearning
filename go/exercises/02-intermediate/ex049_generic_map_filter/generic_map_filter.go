// Package genericmapfilter — Exercise 049 (intermediate).
// Goal:   Implement generic Map and Filter functions over slices.
// Drills: generics, type parameters, constraints (any).
package genericmapfilter

// Map applies f to each element of s and returns a new slice of the results.
func Map[T, U any](s []T, f func(T) U) []U {
	panic("TODO: implement Map")
}

// Filter returns a new slice containing only the elements of s for which
// pred returns true.
func Filter[T any](s []T, pred func(T) bool) []T {
	panic("TODO: implement Filter")
}

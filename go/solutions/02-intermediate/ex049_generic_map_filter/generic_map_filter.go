// Package genericmapfilter — Exercise 049 (reference solution).
package genericmapfilter

// Map applies f to each element of s and returns a new slice of the results.
func Map[T, U any](s []T, f func(T) U) []U {
	out := make([]U, len(s))
	for i, v := range s {
		out[i] = f(v)
	}
	return out
}

// Filter returns a new slice containing only the elements of s for which
// pred returns true.
func Filter[T any](s []T, pred func(T) bool) []T {
	out := make([]T, 0, len(s))
	for _, v := range s {
		if pred(v) {
			out = append(out, v)
		}
	}
	return out
}

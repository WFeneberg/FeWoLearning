// Package slicereverse — Exercise 003 (reference solution).
package slicereverse

// Reverse reverses s in place using a two-pointer swap.
func Reverse(s []int) {
	for i, j := 0, len(s)-1; i < j; i, j = i+1, j-1 {
		s[i], s[j] = s[j], s[i]
	}
}

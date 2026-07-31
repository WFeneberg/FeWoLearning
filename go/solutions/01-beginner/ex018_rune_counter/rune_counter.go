// Package runecounter — Exercise 018 (reference solution).
package runecounter

// CountRunesAndBytes returns the rune count and byte length of s.
func CountRunesAndBytes(s string) (int, int) {
	return len([]rune(s)), len(s)
}

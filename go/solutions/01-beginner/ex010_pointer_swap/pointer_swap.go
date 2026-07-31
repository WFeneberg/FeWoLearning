// Package pointerswap — Exercise 010 (reference solution).
package pointerswap

// Swap exchanges the values stored at a and b.
func Swap(a, b *int) {
	*a, *b = *b, *a
}

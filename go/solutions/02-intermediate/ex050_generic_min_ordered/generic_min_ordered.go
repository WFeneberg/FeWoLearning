// Package genericminordered — Exercise 050 (reference solution).
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
	if len(vals) == 0 {
		panic("Min: no values provided")
	}
	min := vals[0]
	for _, v := range vals[1:] {
		if v < min {
			min = v
		}
	}
	return min
}

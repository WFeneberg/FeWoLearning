// Package matrixtranspose — Exercise 024 (reference solution).
package matrixtranspose

// Transpose returns the transpose of the rectangular matrix m.
func Transpose(m [][]int) [][]int {
	if len(m) == 0 {
		return [][]int{}
	}
	rows := len(m)
	cols := len(m[0])

	result := make([][]int, cols)
	for c := 0; c < cols; c++ {
		result[c] = make([]int, rows)
		for r := 0; r < rows; r++ {
			result[c][r] = m[r][c]
		}
	}
	return result
}

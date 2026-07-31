// Package arraysum — Exercise 004 (reference solution).
package arraysum

func Sum(arr [5]int) int {
	total := 0
	for _, v := range arr {
		total += v
	}
	return total
}

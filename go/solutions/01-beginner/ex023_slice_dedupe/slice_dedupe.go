// Package slicededupe — Exercise 023 (reference solution).
package slicededupe

func Dedupe(nums []int) []int {
	seen := make(map[int]bool, len(nums))
	result := make([]int, 0, len(nums))
	for _, n := range nums {
		if !seen[n] {
			seen[n] = true
			result = append(result, n)
		}
	}
	return result
}

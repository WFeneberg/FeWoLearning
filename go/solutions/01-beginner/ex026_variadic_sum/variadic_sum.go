// Package variadicsum — Exercise 026 (reference solution).
package variadicsum

// SumAndCount returns the sum and count of the given ints.
func SumAndCount(nums ...int) (sum, count int) {
	for _, n := range nums {
		sum += n
	}
	return sum, len(nums)
}

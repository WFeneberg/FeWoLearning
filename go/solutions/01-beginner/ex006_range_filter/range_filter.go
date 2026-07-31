// Package rangefilter — Exercise 006 (reference solution).
package rangefilter

// FilterEven returns a new slice containing only the even numbers from
// nums, preserving their original order.
func FilterEven(nums []int) []int {
	result := []int{}
	for _, n := range nums {
		if n%2 == 0 {
			result = append(result, n)
		}
	}
	return result
}

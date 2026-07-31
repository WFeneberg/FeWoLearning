// Package minmax — Exercise 007 (reference solution).
package minmax

// MinMax returns the minimum and maximum values found in nums.
func MinMax(nums []int) (min, max int) {
	min, max = nums[0], nums[0]
	for _, n := range nums[1:] {
		if n < min {
			min = n
		}
		if n > max {
			max = n
		}
	}
	return min, max
}

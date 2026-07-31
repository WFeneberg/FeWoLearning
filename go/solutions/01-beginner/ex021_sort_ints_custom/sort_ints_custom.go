// Package sortintscustom — Exercise 021 (reference solution).
package sortintscustom

import "sort"

// DescendingInts is a slice of ints that can be sorted in descending
// order via sort.Sort.
type DescendingInts []int

func (d DescendingInts) Len() int {
	return len(d)
}

func (d DescendingInts) Less(i, j int) bool {
	return d[i] > d[j]
}

func (d DescendingInts) Swap(i, j int) {
	d[i], d[j] = d[j], d[i]
}

// SortDescending sorts nums in place in descending order using
// sort.Sort and the DescendingInts type, returning the same slice.
func SortDescending(nums []int) []int {
	sort.Sort(DescendingInts(nums))
	return nums
}

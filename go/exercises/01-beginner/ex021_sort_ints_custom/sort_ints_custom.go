// Package sortintscustom — Exercise 021 (beginner).
// Goal:   implement a descendingInts type satisfying sort.Interface
//         (Len/Less/Swap) and sort it with sort.Sort so the slice ends
//         up in descending order.
// Drills: sort.Interface, method sets, sort.Sort.
package sortintscustom

// DescendingInts is a slice of ints that can be sorted in descending
// order via sort.Sort.
type DescendingInts []int

// Len returns the number of elements in the collection.
func (d DescendingInts) Len() int {
	panic("TODO: implement Len")
}

// Less reports whether the element at index i should sort before the
// element at index j, for descending order.
func (d DescendingInts) Less(i, j int) bool {
	panic("TODO: implement Less")
}

// Swap swaps the elements at indexes i and j.
func (d DescendingInts) Swap(i, j int) {
	panic("TODO: implement Swap")
}

// SortDescending sorts nums in place in descending order using
// sort.Sort and the DescendingInts type, returning the same slice.
func SortDescending(nums []int) []int {
	panic("TODO: implement SortDescending")
}

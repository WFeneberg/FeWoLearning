// Package heapklargest — Exercise 084 (advanced).
// Goal:   Find the k largest values in a slice using a min-heap of size k
//         built with container/heap, in O(n log k) time.
// Drills: container/heap, heap.Interface, bounded min-heap selection.
package heapklargest

// KLargest returns the k largest values from nums, in no particular order.
// If k <= 0 it returns an empty slice. If k >= len(nums) it returns all of
// nums (in no particular order).
func KLargest(nums []int, k int) []int {
	panic("TODO: implement KLargest")
}

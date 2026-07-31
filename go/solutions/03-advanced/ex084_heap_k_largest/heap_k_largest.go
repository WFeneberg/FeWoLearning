// Package heapklargest — Exercise 084 (reference solution).
package heapklargest

import "container/heap"

// intMinHeap is a min-heap of ints implementing heap.Interface.
type intMinHeap []int

func (h intMinHeap) Len() int            { return len(h) }
func (h intMinHeap) Less(i, j int) bool  { return h[i] < h[j] }
func (h intMinHeap) Swap(i, j int)       { h[i], h[j] = h[j], h[i] }
func (h *intMinHeap) Push(x interface{}) { *h = append(*h, x.(int)) }
func (h *intMinHeap) Pop() interface{} {
	old := *h
	n := len(old)
	v := old[n-1]
	*h = old[:n-1]
	return v
}

// KLargest returns the k largest values from nums, in no particular order.
// If k <= 0 it returns an empty slice. If k >= len(nums) it returns all of
// nums (in no particular order).
//
// It maintains a min-heap of at most k elements: for each number, if the
// heap has fewer than k elements it is pushed; otherwise, if the number is
// larger than the current heap minimum, the minimum is popped and the
// number is pushed. This runs in O(n log k) time and O(k) extra space.
func KLargest(nums []int, k int) []int {
	if k <= 0 || len(nums) == 0 {
		return []int{}
	}
	if k > len(nums) {
		k = len(nums)
	}

	h := make(intMinHeap, 0, k)
	heap.Init(&h)

	for _, n := range nums {
		if h.Len() < k {
			heap.Push(&h, n)
			continue
		}
		if n > h[0] {
			heap.Pop(&h)
			heap.Push(&h, n)
		}
	}

	result := make([]int, len(h))
	copy(result, h)
	return result
}

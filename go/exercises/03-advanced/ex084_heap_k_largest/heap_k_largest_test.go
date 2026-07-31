package heapklargest

import (
	"sort"
	"testing"
)

// sortedCopy returns a sorted ascending copy of nums.
func sortedCopy(nums []int) []int {
	out := make([]int, len(nums))
	copy(out, nums)
	sort.Ints(out)
	return out
}

func TestKLargest(t *testing.T) {
	tests := []struct {
		name string
		nums []int
		k    int
		want []int // expected multiset of k largest, sorted ascending
	}{
		{
			name: "basic unordered",
			nums: []int{3, 1, 4, 1, 5, 9, 2, 6},
			k:    3,
			want: []int{5, 6, 9},
		},
		{
			name: "k equals length",
			nums: []int{7, -2, 4},
			k:    3,
			want: []int{-2, 4, 7},
		},
		{
			name: "k greater than length returns all",
			nums: []int{2, 1},
			k:    5,
			want: []int{1, 2},
		},
		{
			name: "k is zero",
			nums: []int{1, 2, 3},
			k:    0,
			want: []int{},
		},
		{
			name: "negative k",
			nums: []int{1, 2, 3},
			k:    -2,
			want: []int{},
		},
		{
			name: "duplicates near the boundary",
			nums: []int{5, 5, 5, 1, 1},
			k:    2,
			want: []int{5, 5},
		},
		{
			name: "negative numbers",
			nums: []int{-5, -1, -10, -3, -2},
			k:    2,
			want: []int{-2, -1},
		},
		{
			name: "single element k1",
			nums: []int{42},
			k:    1,
			want: []int{42},
		},
		{
			name: "empty input",
			nums: []int{},
			k:    3,
			want: []int{},
		},
		{
			name: "larger set",
			nums: []int{10, 4, 3, 50, 23, 8, 90, 1, 77, 34},
			k:    4,
			want: []int{34, 50, 77, 90},
		},
	}

	for _, tc := range tests {
		t.Run(tc.name, func(t *testing.T) {
			got := KLargest(tc.nums, tc.k)
			if len(got) != len(tc.want) {
				t.Fatalf("KLargest(%v, %d) = %v (len %d), want len %d (%v)",
					tc.nums, tc.k, got, len(got), len(tc.want), tc.want)
			}
			gotSorted := sortedCopy(got)
			for i := range gotSorted {
				if gotSorted[i] != tc.want[i] {
					t.Fatalf("KLargest(%v, %d) = %v -> sorted %v, want %v",
						tc.nums, tc.k, got, gotSorted, tc.want)
				}
			}

			// Also verify original input was not mutated.
			// (KLargest must not reorder or modify the caller's slice.)
		})
	}
}

func TestKLargestDoesNotMutateInput(t *testing.T) {
	nums := []int{3, 1, 4, 1, 5}
	orig := sortedCopy(nums)
	_ = KLargest(nums, 2)
	if got := sortedCopy(nums); len(got) != len(orig) {
		t.Fatalf("input length changed: got %v want %v", got, orig)
	} else {
		for i := range got {
			if got[i] != orig[i] {
				t.Fatalf("input mutated: got %v want %v", nums, orig)
			}
		}
	}
}

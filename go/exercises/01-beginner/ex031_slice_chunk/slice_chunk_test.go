package slicechunk

import (
	"reflect"
	"testing"
)

func TestChunk(t *testing.T) {
	cases := []struct {
		name string
		nums []int
		size int
		want [][]int
	}{
		{
			name: "evenly divisible",
			nums: []int{1, 2, 3, 4, 5, 6},
			size: 2,
			want: [][]int{{1, 2}, {3, 4}, {5, 6}},
		},
		{
			name: "shorter final chunk",
			nums: []int{1, 2, 3, 4, 5},
			size: 2,
			want: [][]int{{1, 2}, {3, 4}, {5}},
		},
		{
			name: "size larger than slice",
			nums: []int{1, 2, 3},
			size: 5,
			want: [][]int{{1, 2, 3}},
		},
		{
			name: "empty input",
			nums: []int{},
			size: 3,
			want: [][]int{},
		},
	}

	for _, tc := range cases {
		t.Run(tc.name, func(t *testing.T) {
			got := Chunk(tc.nums, tc.size)
			if len(got) != len(tc.want) {
				t.Fatalf("Chunk(%v, %d) returned %d chunks, want %d", tc.nums, tc.size, len(got), len(tc.want))
			}
			if !reflect.DeepEqual(got, tc.want) {
				t.Errorf("Chunk(%v, %d) = %v, want %v", tc.nums, tc.size, got, tc.want)
			}
		})
	}
}

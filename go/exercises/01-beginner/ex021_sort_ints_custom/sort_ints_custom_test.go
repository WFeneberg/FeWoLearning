package sortintscustom

import (
	"reflect"
	"testing"
)

func TestSortDescending(t *testing.T) {
	cases := []struct {
		name  string
		input []int
		want  []int
	}{
		{"unsorted", []int{3, 1, 4, 1, 5, 9, 2, 6}, []int{9, 6, 5, 4, 3, 2, 1, 1}},
		{"already descending", []int{5, 4, 3, 2, 1}, []int{5, 4, 3, 2, 1}},
		{"ascending", []int{1, 2, 3, 4, 5}, []int{5, 4, 3, 2, 1}},
		{"single", []int{42}, []int{42}},
		{"empty", []int{}, []int{}},
		{"with negatives", []int{-3, 0, 7, -1}, []int{7, 0, -1, -3}},
	}

	for _, tc := range cases {
		t.Run(tc.name, func(t *testing.T) {
			got := SortDescending(tc.input)
			if !reflect.DeepEqual(got, tc.want) {
				t.Errorf("SortDescending(%v) = %v, want %v", tc.input, got, tc.want)
			}
		})
	}
}

func TestDescendingIntsInterface(t *testing.T) {
	d := DescendingInts{1, 3, 2}
	if d.Len() != 3 {
		t.Fatalf("Len() = %d, want 3", d.Len())
	}
	if !d.Less(1, 2) { // 3 > 2, so index 1 ("before" index 2) in descending order
		t.Errorf("Less(1, 2) = false, want true (3 should sort before 2)")
	}
	if d.Less(2, 1) {
		t.Errorf("Less(2, 1) = true, want false (2 should not sort before 3)")
	}
	d.Swap(0, 2)
	want := DescendingInts{2, 3, 1}
	if !reflect.DeepEqual(d, want) {
		t.Errorf("after Swap(0, 2) = %v, want %v", d, want)
	}
}

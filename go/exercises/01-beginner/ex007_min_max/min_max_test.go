package minmax

import "testing"

func TestMinMax(t *testing.T) {
	cases := []struct {
		name    string
		nums    []int
		wantMin int
		wantMax int
	}{
		{"single element", []int{42}, 42, 42},
		{"ascending", []int{1, 2, 3, 4, 5}, 1, 5},
		{"descending", []int{5, 4, 3, 2, 1}, 1, 5},
		{"duplicates", []int{3, 3, 3, 3}, 3, 3},
		{"negatives", []int{-5, -1, -10, 0, 7}, -10, 7},
		{"unordered with duplicates", []int{4, 1, 4, 9, 1, 9, 2}, 1, 9},
	}

	for _, c := range cases {
		t.Run(c.name, func(t *testing.T) {
			gotMin, gotMax := MinMax(c.nums)
			if gotMin != c.wantMin || gotMax != c.wantMax {
				t.Errorf("MinMax(%v) = (%d, %d), want (%d, %d)", c.nums, gotMin, gotMax, c.wantMin, c.wantMax)
			}
		})
	}
}

package variadicsum

import "testing"

func TestSumAndCount(t *testing.T) {
	cases := []struct {
		name      string
		nums      []int
		wantSum   int
		wantCount int
	}{
		{"none", []int{}, 0, 0},
		{"one", []int{7}, 7, 1},
		{"many", []int{1, 2, 3, 4, 5}, 15, 5},
		{"negatives", []int{-3, 3, 10}, 10, 3},
	}
	for _, c := range cases {
		t.Run(c.name, func(t *testing.T) {
			gotSum, gotCount := SumAndCount(c.nums...)
			if gotSum != c.wantSum || gotCount != c.wantCount {
				t.Errorf("SumAndCount(%v) = (%d, %d), want (%d, %d)", c.nums, gotSum, gotCount, c.wantSum, c.wantCount)
			}
		})
	}
}

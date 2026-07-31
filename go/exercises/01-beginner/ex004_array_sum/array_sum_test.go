package arraysum

import "testing"

func TestSum(t *testing.T) {
	cases := []struct {
		arr  [5]int
		want int
	}{
		{[5]int{1, 2, 3, 4, 5}, 15},
		{[5]int{0, 0, 0, 0, 0}, 0},
		{[5]int{-1, -2, -3, -4, -5}, -15},
		{[5]int{10, -5, 3, -2, 4}, 10},
		{[5]int{100, 200, 300, 400, 500}, 1500},
	}
	for _, c := range cases {
		if got := Sum(c.arr); got != c.want {
			t.Errorf("Sum(%v) = %d, want %d", c.arr, got, c.want)
		}
	}
}

package genericminordered

import "testing"

func TestMinInt(t *testing.T) {
	cases := []struct {
		vals []int
		want int
	}{
		{[]int{3, 1, 2}, 1},
		{[]int{-5, -1, -10, 4}, -10},
		{[]int{7}, 7},
		{[]int{2, 2, 2}, 2},
	}
	for _, c := range cases {
		if got := Min(c.vals...); got != c.want {
			t.Errorf("Min(%v) = %d, want %d", c.vals, got, c.want)
		}
	}
}

func TestMinFloat64(t *testing.T) {
	cases := []struct {
		vals []float64
		want float64
	}{
		{[]float64{3.5, 1.2, 2.8}, 1.2},
		{[]float64{-5.5, -1.1, -10.25, 4.0}, -10.25},
		{[]float64{7.7}, 7.7},
		{[]float64{0.0, -0.0}, 0.0},
	}
	for _, c := range cases {
		if got := Min(c.vals...); got != c.want {
			t.Errorf("Min(%v) = %v, want %v", c.vals, got, c.want)
		}
	}
}

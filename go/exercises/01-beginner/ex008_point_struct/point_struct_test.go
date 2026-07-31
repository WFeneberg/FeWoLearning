package pointstruct

import "testing"

func TestDistance(t *testing.T) {
	cases := []struct {
		name string
		a    Point
		b    Point
		want float64
	}{
		{"same point", Point{0, 0}, Point{0, 0}, 0},
		{"3-4-5 triangle", Point{0, 0}, Point{3, 4}, 5},
		{"horizontal", Point{1, 1}, Point{4, 1}, 3},
		{"vertical", Point{2, 5}, Point{2, 1}, 4},
		{"negative coords", Point{-1, -1}, Point{2, 3}, 5},
	}
	for _, c := range cases {
		if got := c.a.Distance(c.b); got != c.want {
			t.Errorf("%s: Distance(%v, %v) = %v, want %v", c.name, c.a, c.b, got, c.want)
		}
	}
}

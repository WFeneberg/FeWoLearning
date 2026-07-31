package pointerswap

import "testing"

func TestSwap(t *testing.T) {
	cases := []struct {
		a, b         int
		wantA, wantB int
	}{
		{1, 2, 2, 1},
		{-5, 5, 5, -5},
		{0, 0, 0, 0},
		{100, -100, -100, 100},
	}
	for _, c := range cases {
		a, b := c.a, c.b
		Swap(&a, &b)
		if a != c.wantA || b != c.wantB {
			t.Errorf("Swap(%d, %d) = (%d, %d), want (%d, %d)", c.a, c.b, a, b, c.wantA, c.wantB)
		}
	}
}

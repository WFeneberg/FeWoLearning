package dividesafe

import "testing"

func TestDivide(t *testing.T) {
	cases := []struct {
		a, b    int
		want    int
		wantErr bool
	}{
		{10, 2, 5, false},
		{9, 3, 3, false},
		{7, 2, 3, false},
		{-8, 4, -2, false},
		{5, 0, 0, true},
	}
	for _, c := range cases {
		got, err := Divide(c.a, c.b)
		if c.wantErr {
			if err == nil {
				t.Errorf("Divide(%d, %d) expected an error, got nil", c.a, c.b)
			}
			continue
		}
		if err != nil {
			t.Errorf("Divide(%d, %d) unexpected error: %v", c.a, c.b, err)
		}
		if got != c.want {
			t.Errorf("Divide(%d, %d) = %d, want %d", c.a, c.b, got, c.want)
		}
	}
}

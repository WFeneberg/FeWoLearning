package rectanglearea

import "testing"

func TestRectangleAreaAndPerimeter(t *testing.T) {
	cases := []struct {
		width, height     float64
		wantArea, wantPer float64
	}{
		{2, 3, 6, 10},
		{5, 5, 25, 20},
		{1, 10, 10, 22},
		{7.5, 2, 15, 19},
	}

	for _, c := range cases {
		r := Rectangle{Width: c.width, Height: c.height}
		if got := r.Area(); got != c.wantArea {
			t.Errorf("Rectangle{%v,%v}.Area() = %v, want %v", c.width, c.height, got, c.wantArea)
		}
		if got := r.Perimeter(); got != c.wantPer {
			t.Errorf("Rectangle{%v,%v}.Perimeter() = %v, want %v", c.width, c.height, got, c.wantPer)
		}
	}
}

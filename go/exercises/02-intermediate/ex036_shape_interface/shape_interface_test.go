package shapeinterface

import (
	"math"
	"testing"
)

func TestCircleArea(t *testing.T) {
	c := Circle{Radius: 2}
	want := math.Pi * 4
	if got := c.Area(); math.Abs(got-want) > 1e-9 {
		t.Errorf("Circle{2}.Area() = %v, want %v", got, want)
	}
}

func TestSquareArea(t *testing.T) {
	s := Square{Side: 3}
	want := 9.0
	if got := s.Area(); got != want {
		t.Errorf("Square{3}.Area() = %v, want %v", got, want)
	}
}

func TestTotalArea(t *testing.T) {
	shapes := []Shape{
		Circle{Radius: 1},
		Square{Side: 2},
		Square{Side: 3},
		Circle{Radius: 2},
	}
	want := math.Pi*1 + 4 + 9 + math.Pi*4
	got := TotalArea(shapes)
	if math.Abs(got-want) > 1e-9 {
		t.Errorf("TotalArea(shapes) = %v, want %v", got, want)
	}
}

func TestTotalAreaEmpty(t *testing.T) {
	if got := TotalArea(nil); got != 0 {
		t.Errorf("TotalArea(nil) = %v, want 0", got)
	}
}

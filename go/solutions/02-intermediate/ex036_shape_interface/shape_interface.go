// Package shapeinterface — Exercise 036 (reference solution).
package shapeinterface

import "math"

// Shape is satisfied by any type that can report its own area.
type Shape interface {
	Area() float64
}

// Circle is a shape defined by its radius.
type Circle struct {
	Radius float64
}

// Area returns the area of the circle.
func (c Circle) Area() float64 {
	return math.Pi * c.Radius * c.Radius
}

// Square is a shape defined by its side length.
type Square struct {
	Side float64
}

// Area returns the area of the square.
func (s Square) Area() float64 {
	return s.Side * s.Side
}

// TotalArea sums the Area() of every shape in shapes.
func TotalArea(shapes []Shape) float64 {
	total := 0.0
	for _, sh := range shapes {
		total += sh.Area()
	}
	return total
}

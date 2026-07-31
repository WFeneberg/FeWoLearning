// Package shapeinterface — Exercise 036 (intermediate).
// Goal:   Define a Shape interface with Area() float64 satisfied by Circle
//         and Square types, then sum areas across mixed implementations.
// Drills: interfaces, interface satisfaction, polymorphism via slices.
package shapeinterface

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
	panic("TODO: implement Circle.Area")
}

// Square is a shape defined by its side length.
type Square struct {
	Side float64
}

// Area returns the area of the square.
func (s Square) Area() float64 {
	panic("TODO: implement Square.Area")
}

// TotalArea sums the Area() of every shape in shapes.
func TotalArea(shapes []Shape) float64 {
	panic("TODO: implement TotalArea")
}

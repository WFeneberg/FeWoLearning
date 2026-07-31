// Package rectanglearea — Exercise 009 (beginner).
// Goal:   Define a Rectangle struct with Width/Height and implement
//         Area() and Perimeter() methods on it.
// Drills: structs, methods, value receivers.
package rectanglearea

// Rectangle represents a rectangle with a width and height.
type Rectangle struct {
	Width  float64
	Height float64
}

// Area returns the area of the rectangle.
func (r Rectangle) Area() float64 {
	panic("TODO: implement Area")
}

// Perimeter returns the perimeter of the rectangle.
func (r Rectangle) Perimeter() float64 {
	panic("TODO: implement Perimeter")
}

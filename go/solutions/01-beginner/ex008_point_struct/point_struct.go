// Package pointstruct — Exercise 008 (reference solution).
package pointstruct

import "math"

// Point represents a location in 2D space.
type Point struct {
	X float64
	Y float64
}

// Distance returns the Euclidean distance between p and other.
func (p Point) Distance(other Point) float64 {
	dx := p.X - other.X
	dy := p.Y - other.Y
	return math.Sqrt(dx*dx + dy*dy)
}

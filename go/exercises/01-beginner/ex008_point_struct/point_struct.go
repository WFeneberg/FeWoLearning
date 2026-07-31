// Package pointstruct — Exercise 008 (beginner).
// Goal:   Define a Point struct with X, Y float64 fields and a Distance
//         method computing the Euclidean distance between two points.
// Drills: struct definition, methods, math.Sqrt.
package pointstruct

// Point represents a location in 2D space.
type Point struct {
	X float64
	Y float64
}

// Distance returns the Euclidean distance between p and other.
func (p Point) Distance(other Point) float64 {
	panic("TODO: implement Distance")
}

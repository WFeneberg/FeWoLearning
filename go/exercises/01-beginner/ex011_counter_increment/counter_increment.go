// Package counterincrement — Exercise 011 (beginner).
// Goal:   Define a Counter struct with an Increment() method using a
//         pointer receiver to mutate an internal count field.
// Drills: structs, pointer receivers, methods.
package counterincrement

// Counter holds a running count.
type Counter struct {
	count int
}

// Increment increases the counter's internal count by one.
func (c *Counter) Increment() {
	panic("TODO: implement Increment")
}

// Value returns the current count.
func (c *Counter) Value() int {
	panic("TODO: implement Value")
}

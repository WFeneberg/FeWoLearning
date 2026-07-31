// Package counterincrement — Exercise 011 (reference solution).
package counterincrement

// Counter holds a running count.
type Counter struct {
	count int
}

// Increment increases the counter's internal count by one.
func (c *Counter) Increment() {
	c.count++
}

// Value returns the current count.
func (c *Counter) Value() int {
	return c.count
}

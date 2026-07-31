// Package ioreadercounter — Exercise 054 (intermediate).
// Goal:   Implement a CountingReader that wraps an io.Reader and tracks the
//         total number of bytes read through it.
// Drills: io.Reader interface, embedding/composition, error propagation.
package ioreadercounter

import "io"

// CountingReader wraps an io.Reader and counts the total bytes read.
type CountingReader struct {
	R     io.Reader
	Count int64
}

// NewCountingReader returns a *CountingReader wrapping r.
func NewCountingReader(r io.Reader) *CountingReader {
	panic("TODO: implement NewCountingReader")
}

// Read implements io.Reader, delegating to the wrapped reader and
// accumulating the number of bytes successfully read into Count.
func (c *CountingReader) Read(p []byte) (int, error) {
	panic("TODO: implement Read")
}

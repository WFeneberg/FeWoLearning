// Package ioreadercounter — Exercise 054 (reference solution).
package ioreadercounter

import "io"

// CountingReader wraps an io.Reader and counts the total bytes read.
type CountingReader struct {
	R     io.Reader
	Count int64
}

// NewCountingReader returns a *CountingReader wrapping r.
func NewCountingReader(r io.Reader) *CountingReader {
	return &CountingReader{R: r}
}

// Read implements io.Reader, delegating to the wrapped reader and
// accumulating the number of bytes successfully read into Count.
func (c *CountingReader) Read(p []byte) (int, error) {
	n, err := c.R.Read(p)
	c.Count += int64(n)
	return n, err
}

// Package iowritermultiplex — Exercise 055 (reference solution).
package iowritermultiplex

import "io"

// MultiplexWriter is an io.Writer that fans out every Write call to a set
// of underlying writers.
type MultiplexWriter struct {
	writers []io.Writer
}

// NewMultiplexWriter creates a MultiplexWriter that writes to all of ws.
func NewMultiplexWriter(ws ...io.Writer) *MultiplexWriter {
	return &MultiplexWriter{writers: ws}
}

// Write writes p to every underlying writer. It returns the number of bytes
// written (len(p) on success) and the first error encountered, if any. If a
// short write occurs on any destination, Write returns io.ErrShortWrite.
func (m *MultiplexWriter) Write(p []byte) (int, error) {
	for _, w := range m.writers {
		n, err := w.Write(p)
		if err != nil {
			return n, err
		}
		if n != len(p) {
			return n, io.ErrShortWrite
		}
	}
	return len(p), nil
}

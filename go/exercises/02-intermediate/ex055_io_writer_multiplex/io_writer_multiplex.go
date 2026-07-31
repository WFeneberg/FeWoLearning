// Package iowritermultiplex — Exercise 055 (intermediate).
// Goal:   Implement a MultiplexWriter that writes each Write call's data to
//         several underlying io.Writers, mirroring writes to every destination.
// Drills: io.Writer interface, composition, error aggregation.
package iowritermultiplex

import "io"

// MultiplexWriter is an io.Writer that fans out every Write call to a set
// of underlying writers.
type MultiplexWriter struct {
	writers []io.Writer
}

// NewMultiplexWriter creates a MultiplexWriter that writes to all of ws.
func NewMultiplexWriter(ws ...io.Writer) *MultiplexWriter {
	panic("TODO: implement NewMultiplexWriter")
}

// Write writes p to every underlying writer. It returns the number of bytes
// written (len(p) on success) and the first error encountered, if any. If a
// short write occurs on any destination, Write returns io.ErrShortWrite.
func (m *MultiplexWriter) Write(p []byte) (int, error) {
	panic("TODO: implement Write")
}

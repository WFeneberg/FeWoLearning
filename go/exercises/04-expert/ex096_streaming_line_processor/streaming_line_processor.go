// Package streaminglineprocessor — Exercise 096 (expert).
// Goal:   Stream text line-by-line from r through transform to w using a
//         bounded channel pipeline so that a slow writer applies backpressure
//         to the reader instead of buffering the whole input in memory.
// Drills: pipeline concurrency, bounded channels as backpressure, goroutine
//         lifetime management, error propagation across pipeline stages.
package streaminglineprocessor

import "io"

// bufferSize is the capacity of the bounded channels between the pipeline
// stages. A slow writer causes these buffers to fill, which in turn blocks
// the upstream stages (backpressure) instead of reading unboundedly ahead.
const bufferSize = 8

// ProcessStream reads r line-by-line, applies transform to each line, and
// writes the results to w (one per line, newline-terminated), preserving the
// original order. Reading, transforming, and writing happen concurrently in
// a pipeline connected by bounded channels, so a slow w naturally throttles
// how far ahead the reading of r is allowed to get.
//
// ProcessStream returns the first error encountered while reading r or
// writing w (whichever happens first in pipeline order); it returns nil if
// the entire stream was processed successfully.
func ProcessStream(r io.Reader, transform func(string) string, w io.Writer) error {
	panic("TODO: implement ProcessStream")
}

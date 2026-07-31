// Package streaminglineprocessor — Exercise 096 (reference solution).
package streaminglineprocessor

import (
	"bufio"
	"fmt"
	"io"
)

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
	lines := make(chan string, bufferSize)
	results := make(chan string, bufferSize)
	scanErrCh := make(chan error, 1)

	// Stage 1 (reader): scan r line-by-line and push onto the bounded
	// "lines" channel. Once the channel is full, this goroutine blocks on
	// the send, which is exactly the backpressure the caller relies on: a
	// slow downstream stage stalls how far ahead reading of r can get,
	// instead of buffering the whole input in memory.
	go func() {
		defer close(lines)
		scanner := bufio.NewScanner(r)
		scanner.Buffer(make([]byte, 0, 64*1024), 1024*1024)
		for scanner.Scan() {
			lines <- scanner.Text()
		}
		scanErrCh <- scanner.Err()
	}()

	// Stage 2 (transform): apply transform to each line and push onto the
	// bounded "results" channel, again applying backpressure upstream when
	// the writer falls behind.
	go func() {
		defer close(results)
		for line := range lines {
			results <- transform(line)
		}
	}()

	// Stage 3 (writer): runs on the calling goroutine so that ProcessStream
	// only returns once every line has actually been written (or an error
	// has occurred). We keep draining "results" even after a write error so
	// stages 1 and 2 are never left blocked sending into a channel nobody
	// reads from anymore (no goroutine leak).
	var writeErr error
	for line := range results {
		if writeErr != nil {
			continue
		}
		if _, err := fmt.Fprintln(w, line); err != nil {
			writeErr = err
		}
	}

	scanErr := <-scanErrCh

	if writeErr != nil {
		return writeErr
	}
	return scanErr
}

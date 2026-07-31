// Package bufiowriterflush — Exercise 057 (reference solution).
package bufiowriterflush

import (
	"bufio"
	"io"
)

// WriteBuffered wraps w in a *bufio.Writer, writes each of lines followed by
// a newline into the buffer, and returns the *bufio.Writer WITHOUT flushing
// it. The underlying writer w must receive no bytes until the caller invokes
// Flush() on the returned *bufio.Writer.
func WriteBuffered(w io.Writer, lines []string) *bufio.Writer {
	bw := bufio.NewWriter(w)
	for _, line := range lines {
		bw.WriteString(line)
		bw.WriteByte('\n')
	}
	return bw
}

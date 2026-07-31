// Package bufiowriterflush — Exercise 057 (intermediate).
// Goal:   Write a slice of lines through a bufio.Writer without flushing
//         until the caller explicitly calls Flush on the returned writer.
// Drills: bufio.Writer, manual flush control, io.Writer.
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
	panic("TODO: implement WriteBuffered")
}

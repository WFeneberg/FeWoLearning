// Package filelinereader — Exercise 022 (beginner).
// Goal:   Read all lines from an io.Reader into a slice of strings.
// Drills: basic I/O, io.Reader, bufio.Scanner.
package filelinereader

import "io"

// ReadLines reads all lines from r and returns them as a slice of strings,
// without the trailing newline characters.
func ReadLines(r io.Reader) ([]string, error) {
	panic("TODO: implement ReadLines")
}

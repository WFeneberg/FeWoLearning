// Package bufiocsvparser — Exercise 056 (intermediate).
// Goal:   Parse simple CSV text from an io.Reader into rows of columns.
// Drills: bufio.Scanner, io.Reader, strings.Split, error handling.
package bufiocsvparser

import "io"

// ParseCSVLines reads r line by line using a bufio.Scanner and splits
// each non-empty line on commas, returning the parsed records.
func ParseCSVLines(r io.Reader) ([][]string, error) {
	panic("TODO: implement ParseCSVLines")
}

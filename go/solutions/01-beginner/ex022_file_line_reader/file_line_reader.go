// Package filelinereader — Exercise 022 (reference solution).
package filelinereader

import (
	"bufio"
	"io"
)

// ReadLines reads all lines from r and returns them as a slice of strings,
// without the trailing newline characters.
func ReadLines(r io.Reader) ([]string, error) {
	var lines []string
	scanner := bufio.NewScanner(r)
	for scanner.Scan() {
		lines = append(lines, scanner.Text())
	}
	if err := scanner.Err(); err != nil {
		return nil, err
	}
	return lines, nil
}

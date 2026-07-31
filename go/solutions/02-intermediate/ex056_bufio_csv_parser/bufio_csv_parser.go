// Package bufiocsvparser — Exercise 056 (reference solution).
package bufiocsvparser

import (
	"bufio"
	"io"
	"strings"
)

// ParseCSVLines reads r line by line using a bufio.Scanner and splits
// each non-empty line on commas, returning the parsed records.
func ParseCSVLines(r io.Reader) ([][]string, error) {
	var records [][]string

	scanner := bufio.NewScanner(r)
	for scanner.Scan() {
		line := scanner.Text()
		if line == "" {
			continue
		}
		records = append(records, strings.Split(line, ","))
	}
	if err := scanner.Err(); err != nil {
		return nil, err
	}

	return records, nil
}

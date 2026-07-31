// Package strtointsum — Exercise 015 (reference solution).
package strtointsum

import "strconv"
import "strings"

// SumCSV parses a comma-separated list of integers in s and returns their
// sum. If any token fails to parse as an integer, SumCSV returns a non-nil
// error.
func SumCSV(s string) (int, error) {
	tokens := strings.Split(s, ",")
	sum := 0
	for _, tok := range tokens {
		n, err := strconv.Atoi(strings.TrimSpace(tok))
		if err != nil {
			return 0, err
		}
		sum += n
	}
	return sum, nil
}

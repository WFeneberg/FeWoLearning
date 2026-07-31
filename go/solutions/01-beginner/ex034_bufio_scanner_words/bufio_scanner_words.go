// Package bufioscannerwords — Exercise 034 (reference solution).
package bufioscannerwords

import (
	"bufio"
	"io"
)

// SplitWords reads all of r and returns the whitespace-separated words in it,
// in the order they appear.
func SplitWords(r io.Reader) []string {
	var words []string
	scanner := bufio.NewScanner(r)
	scanner.Split(bufio.ScanWords)
	for scanner.Scan() {
		words = append(words, scanner.Text())
	}
	return words
}

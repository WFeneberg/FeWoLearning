// Package wordcount — Exercise 005 (reference solution).
package wordcount

import "strings"

func WordCount(text string) map[string]int {
	counts := make(map[string]int)
	for _, word := range strings.Fields(text) {
		counts[word]++
	}
	return counts
}

// Package benchmarkstringconcat — Exercise 087 (advanced).
// Goal:   Compare two string-building strategies — naive "+=" concatenation
//         vs. strings.Builder — and benchmark them with testing.B.
// Drills: testing.B, strings.Builder, allocation-aware string building.
package benchmarkstringconcat

import (
	"fmt"
	"testing"
)

// ConcatPlus builds a single string by repeatedly appending each element of
// words with the "+=" operator (O(n^2) worst case due to repeated copying).
func ConcatPlus(words []string) string {
	panic("TODO: implement ConcatPlus")
}

// ConcatBuilder builds a single string using strings.Builder, which grows an
// internal buffer and avoids the repeated re-allocation "+=" incurs.
func ConcatBuilder(words []string) string {
	panic("TODO: implement ConcatBuilder")
}

// benchWords returns a deterministic slice of words used by the benchmarks
// below. It contains no randomness or I/O so results are reproducible.
func benchWords() []string {
	words := make([]string, 500)
	for i := range words {
		words[i] = fmt.Sprintf("word%d", i)
	}
	return words
}

// BenchmarkConcatPlus measures the "+=" concatenation strategy.
func BenchmarkConcatPlus(b *testing.B) {
	words := benchWords()
	b.ResetTimer()
	for i := 0; i < b.N; i++ {
		ConcatPlus(words)
	}
}

// BenchmarkConcatBuilder measures the strings.Builder strategy.
func BenchmarkConcatBuilder(b *testing.B) {
	words := benchWords()
	b.ResetTimer()
	for i := 0; i < b.N; i++ {
		ConcatBuilder(words)
	}
}

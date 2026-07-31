// Package titlecase — Exercise 016 (reference solution).
package titlecase

import (
	"strings"
	"unicode"
)

// TitleCase returns s with the first letter of each whitespace-separated
// word capitalized.
func TitleCase(s string) string {
	words := strings.Fields(s)
	for i, w := range words {
		r := []rune(w)
		r[0] = unicode.ToUpper(r[0])
		words[i] = string(r)
	}
	return strings.Join(words, " ")
}

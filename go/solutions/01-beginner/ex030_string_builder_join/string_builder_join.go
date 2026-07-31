// Package stringbuilderjoin — Exercise 030 (reference solution).
package stringbuilderjoin

import "strings"

func JoinWithBuilder(parts []string, sep string) string {
	var b strings.Builder
	for i, p := range parts {
		if i > 0 {
			b.WriteString(sep)
		}
		b.WriteString(p)
	}
	return b.String()
}

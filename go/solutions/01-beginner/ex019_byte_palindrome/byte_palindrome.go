// Package bytepalindrome — Exercise 019 (reference solution).
package bytepalindrome

// IsPalindrome reports whether s reads the same forwards and backwards,
// treating upper- and lower-case letters as equal.
func IsPalindrome(s string) bool {
	b := []byte(s)
	i, j := 0, len(b)-1
	for i < j {
		bi, bj := toLower(b[i]), toLower(b[j])
		if bi != bj {
			return false
		}
		i++
		j--
	}
	return true
}

// toLower folds an ASCII uppercase byte to lowercase; other bytes pass through.
func toLower(c byte) byte {
	if c >= 'A' && c <= 'Z' {
		return c + ('a' - 'A')
	}
	return c
}

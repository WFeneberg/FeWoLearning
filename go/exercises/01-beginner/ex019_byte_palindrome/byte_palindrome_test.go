package bytepalindrome

import "testing"

func TestIsPalindrome(t *testing.T) {
	cases := map[string]bool{
		"":         true,
		"a":        true,
		"aa":       true,
		"ab":       false,
		"racecar":  true,
		"hello":    false,
		"Racecar":  true,
		"Was it a car or a cat I saw": false,
		"Level":    true,
		"noon":     true,
		"golang":   false,
	}
	for s, want := range cases {
		if got := IsPalindrome(s); got != want {
			t.Errorf("IsPalindrome(%q) = %v, want %v", s, got, want)
		}
	}
}

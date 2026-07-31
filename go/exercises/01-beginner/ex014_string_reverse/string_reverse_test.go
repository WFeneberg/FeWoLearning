package stringreverse

import "testing"

func TestReverseString(t *testing.T) {
	cases := map[string]string{
		"hello":     "olleh",
		"Go":        "oG",
		"a":         "a",
		"":          "",
		"héllo":     "olléh",
		"日本語":       "語本日",
		"racecar":   "racecar",
		"go, gopher!": "!rehpog ,og",
	}
	for input, want := range cases {
		if got := ReverseString(input); got != want {
			t.Errorf("ReverseString(%q) = %q, want %q", input, got, want)
		}
	}
}

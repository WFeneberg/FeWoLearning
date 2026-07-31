package titlecase

import "testing"

func TestTitleCase(t *testing.T) {
	cases := map[string]string{
		"hello world":       "Hello World",
		"go is fun":         "Go Is Fun",
		"Already Capital":   "Already Capital",
		"mixed CASE words":  "Mixed CASE Words",
		"single":            "Single",
	}
	for in, want := range cases {
		if got := TitleCase(in); got != want {
			t.Errorf("TitleCase(%q) = %q, want %q", in, got, want)
		}
	}
}

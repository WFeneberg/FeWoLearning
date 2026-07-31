package bufioscannerwords

import (
	"reflect"
	"strings"
	"testing"
)

func TestSplitWords(t *testing.T) {
	cases := []struct {
		name  string
		input string
		want  []string
	}{
		{
			name:  "single line",
			input: "the quick brown fox",
			want:  []string{"the", "quick", "brown", "fox"},
		},
		{
			name:  "multi line",
			input: "hello world\ngo is   fun\n\ntabs\tare\twords too",
			want:  []string{"hello", "world", "go", "is", "fun", "tabs", "are", "words", "too"},
		},
		{
			name:  "empty input",
			input: "",
			want:  nil,
		},
		{
			name:  "only whitespace",
			input: "   \n\t\n  ",
			want:  nil,
		},
	}

	for _, tc := range cases {
		t.Run(tc.name, func(t *testing.T) {
			got := SplitWords(strings.NewReader(tc.input))
			if !reflect.DeepEqual(got, tc.want) {
				t.Errorf("SplitWords(%q) = %v, want %v", tc.input, got, tc.want)
			}
		})
	}
}

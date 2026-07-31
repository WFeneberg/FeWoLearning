package filelinereader

import (
	"reflect"
	"strings"
	"testing"
)

func TestReadLines(t *testing.T) {
	cases := []struct {
		name  string
		input string
		want  []string
	}{
		{
			name:  "multiple lines",
			input: "hello\nworld\nfoo\n",
			want:  []string{"hello", "world", "foo"},
		},
		{
			name:  "no trailing newline",
			input: "one\ntwo",
			want:  []string{"one", "two"},
		},
		{
			name:  "single line",
			input: "solo",
			want:  []string{"solo"},
		},
		{
			name:  "empty input",
			input: "",
			want:  nil,
		},
		{
			name:  "blank lines preserved",
			input: "a\n\nb\n",
			want:  []string{"a", "", "b"},
		},
	}

	for _, tc := range cases {
		t.Run(tc.name, func(t *testing.T) {
			got, err := ReadLines(strings.NewReader(tc.input))
			if err != nil {
				t.Fatalf("ReadLines(%q) returned error: %v", tc.input, err)
			}
			if !reflect.DeepEqual(got, tc.want) {
				t.Errorf("ReadLines(%q) = %#v, want %#v", tc.input, got, tc.want)
			}
		})
	}
}

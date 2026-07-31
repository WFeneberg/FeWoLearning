package wordcount

import (
	"reflect"
	"testing"
)

func TestWordCount(t *testing.T) {
	cases := []struct {
		name string
		text string
		want map[string]int
	}{
		{
			name: "repeated words",
			text: "the quick brown fox the quick fox",
			want: map[string]int{
				"the":   2,
				"quick": 2,
				"brown": 1,
				"fox":   2,
			},
		},
		{
			name: "single word",
			text: "hello",
			want: map[string]int{"hello": 1},
		},
		{
			name: "extra whitespace",
			text: "a  b   a\tb\na",
			want: map[string]int{"a": 3, "b": 2},
		},
		{
			name: "empty string",
			text: "",
			want: map[string]int{},
		},
	}

	for _, tc := range cases {
		t.Run(tc.name, func(t *testing.T) {
			if got := WordCount(tc.text); !reflect.DeepEqual(got, tc.want) {
				t.Errorf("WordCount(%q) = %v, want %v", tc.text, got, tc.want)
			}
		})
	}
}

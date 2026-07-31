package bufiocsvparser

import (
	"reflect"
	"strings"
	"testing"
)

func TestParseCSVLines(t *testing.T) {
	cases := []struct {
		name  string
		input string
		want  [][]string
	}{
		{
			name:  "simple rows",
			input: "a,b,c\n1,2,3\n",
			want: [][]string{
				{"a", "b", "c"},
				{"1", "2", "3"},
			},
		},
		{
			name:  "single column",
			input: "foo\nbar\nbaz",
			want: [][]string{
				{"foo"},
				{"bar"},
				{"baz"},
			},
		},
		{
			name:  "skips blank lines",
			input: "x,y\n\n1,2\n\n",
			want: [][]string{
				{"x", "y"},
				{"1", "2"},
			},
		},
		{
			name:  "no trailing newline",
			input: "name,age\nAlice,30\nBob,25",
			want: [][]string{
				{"name", "age"},
				{"Alice", "30"},
				{"Bob", "25"},
			},
		},
	}

	for _, tc := range cases {
		t.Run(tc.name, func(t *testing.T) {
			got, err := ParseCSVLines(strings.NewReader(tc.input))
			if err != nil {
				t.Fatalf("ParseCSVLines(%q) returned error: %v", tc.input, err)
			}
			if !reflect.DeepEqual(got, tc.want) {
				t.Errorf("ParseCSVLines(%q) = %#v, want %#v", tc.input, got, tc.want)
			}
		})
	}
}

func TestParseCSVLinesEmptyInput(t *testing.T) {
	got, err := ParseCSVLines(strings.NewReader(""))
	if err != nil {
		t.Fatalf("ParseCSVLines(\"\") returned error: %v", err)
	}
	if len(got) != 0 {
		t.Errorf("ParseCSVLines(\"\") = %#v, want empty slice", got)
	}
}

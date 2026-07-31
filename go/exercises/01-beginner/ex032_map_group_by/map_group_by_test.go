package mapgroupby

import (
	"reflect"
	"testing"
)

func TestGroupByLength(t *testing.T) {
	cases := []struct {
		name  string
		words []string
		want  map[int][]string
	}{
		{
			name:  "mixed lengths",
			words: []string{"a", "be", "cat", "dog", "at", "hi"},
			want: map[int][]string{
				1: {"a"},
				2: {"be", "at", "hi"},
				3: {"cat", "dog"},
			},
		},
		{
			name:  "empty input",
			words: []string{},
			want:  map[int][]string{},
		},
		{
			name:  "single word",
			words: []string{"hello"},
			want: map[int][]string{
				5: {"hello"},
			},
		},
		{
			name:  "all same length",
			words: []string{"foo", "bar", "baz"},
			want: map[int][]string{
				3: {"foo", "bar", "baz"},
			},
		},
	}

	for _, tc := range cases {
		t.Run(tc.name, func(t *testing.T) {
			got := GroupByLength(tc.words)
			if !reflect.DeepEqual(got, tc.want) {
				t.Errorf("GroupByLength(%v) = %v, want %v", tc.words, got, tc.want)
			}
		})
	}
}

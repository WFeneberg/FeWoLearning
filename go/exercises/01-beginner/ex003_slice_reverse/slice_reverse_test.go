package slicereverse

import (
	"reflect"
	"testing"
)

func TestReverse(t *testing.T) {
	cases := []struct {
		name string
		in   []int
		want []int
	}{
		{"empty", []int{}, []int{}},
		{"single", []int{1}, []int{1}},
		{"even length", []int{1, 2, 3, 4}, []int{4, 3, 2, 1}},
		{"odd length", []int{1, 2, 3, 4, 5}, []int{5, 4, 3, 2, 1}},
		{"duplicates", []int{7, 7, 3, 7}, []int{7, 3, 7, 7}},
	}

	for _, tc := range cases {
		t.Run(tc.name, func(t *testing.T) {
			s := tc.in
			Reverse(s)
			if !reflect.DeepEqual(s, tc.want) {
				t.Errorf("Reverse(%v) = %v, want %v", tc.name, s, tc.want)
			}
		})
	}
}

func TestReverseInPlace(t *testing.T) {
	s := []int{1, 2, 3, 4, 5}
	orig := s
	Reverse(s)

	if len(s) > 0 && &s[0] != &orig[0] {
		t.Fatalf("Reverse must mutate the underlying array in place, got a different backing array")
	}

	want := []int{5, 4, 3, 2, 1}
	if !reflect.DeepEqual(s, want) {
		t.Errorf("Reverse(%v) = %v, want %v", []int{1, 2, 3, 4, 5}, s, want)
	}
}

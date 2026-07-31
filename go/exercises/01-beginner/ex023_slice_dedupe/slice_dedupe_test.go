package slicededupe

import (
	"reflect"
	"testing"
)

func TestDedupe(t *testing.T) {
	cases := []struct {
		name string
		in   []int
		want []int
	}{
		{"empty", []int{}, []int{}},
		{"no duplicates", []int{1, 2, 3}, []int{1, 2, 3}},
		{"all duplicates", []int{7, 7, 7, 7}, []int{7}},
		{"mixed order", []int{4, 1, 4, 2, 1, 3, 2}, []int{4, 1, 2, 3}},
		{"negative numbers", []int{-1, 0, -1, 2, 0}, []int{-1, 0, 2}},
	}
	for _, c := range cases {
		t.Run(c.name, func(t *testing.T) {
			got := Dedupe(c.in)
			if len(got) != len(c.want) {
				t.Fatalf("Dedupe(%v) = %v, want %v", c.in, got, c.want)
			}
			if len(c.want) > 0 && !reflect.DeepEqual(got, c.want) {
				t.Errorf("Dedupe(%v) = %v, want %v", c.in, got, c.want)
			}
		})
	}
}

package rangefilter

import (
	"reflect"
	"testing"
)

func TestFilterEven(t *testing.T) {
	cases := []struct {
		name string
		in   []int
		want []int
	}{
		{"mixed", []int{1, 2, 3, 4, 5, 6}, []int{2, 4, 6}},
		{"all odd", []int{1, 3, 5, 7}, []int{}},
		{"all even", []int{2, 4, 6}, []int{2, 4, 6}},
		{"negative and zero", []int{-4, -3, -2, -1, 0}, []int{-4, -2, 0}},
		{"empty", []int{}, []int{}},
		{"preserves order", []int{7, 6, 5, 4, 3, 2}, []int{6, 4, 2}},
	}
	for _, tc := range cases {
		t.Run(tc.name, func(t *testing.T) {
			got := FilterEven(tc.in)
			if !reflect.DeepEqual(got, tc.want) {
				t.Errorf("FilterEven(%v) = %v, want %v", tc.in, got, tc.want)
			}
		})
	}
}

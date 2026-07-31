package channelpipeline

import (
	"reflect"
	"testing"
)

func TestPipeline(t *testing.T) {
	cases := []struct {
		name string
		in   []int
		want []int
	}{
		{"empty", []int{}, []int{}},
		{"single", []int{4}, []int{16}},
		{"sequence", []int{1, 2, 3, 4, 5}, []int{1, 4, 9, 16, 25}},
		{"negatives", []int{-2, -1, 0, 1, 2}, []int{4, 1, 0, 1, 4}},
	}

	for _, tc := range cases {
		t.Run(tc.name, func(t *testing.T) {
			got := Pipeline(tc.in)
			if !reflect.DeepEqual(got, tc.want) {
				t.Errorf("Pipeline(%v) = %v, want %v", tc.in, got, tc.want)
			}
		})
	}
}

func TestGenerateAndSquareStages(t *testing.T) {
	gen := Generate([]int{2, 3, 4})
	sq := Square(gen)

	want := []int{4, 9, 16}
	var got []int
	for v := range sq {
		got = append(got, v)
	}

	if !reflect.DeepEqual(got, want) {
		t.Errorf("Square(Generate(...)) = %v, want %v", got, want)
	}
}

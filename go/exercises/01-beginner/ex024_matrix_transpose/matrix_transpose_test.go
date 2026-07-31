package matrixtranspose

import (
	"reflect"
	"testing"
)

func TestTranspose(t *testing.T) {
	cases := []struct {
		name string
		in   [][]int
		want [][]int
	}{
		{
			name: "2x3",
			in: [][]int{
				{1, 2, 3},
				{4, 5, 6},
			},
			want: [][]int{
				{1, 4},
				{2, 5},
				{3, 6},
			},
		},
		{
			name: "square",
			in: [][]int{
				{1, 2},
				{3, 4},
			},
			want: [][]int{
				{1, 3},
				{2, 4},
			},
		},
		{
			name: "single row",
			in: [][]int{
				{1, 2, 3, 4},
			},
			want: [][]int{
				{1}, {2}, {3}, {4},
			},
		},
		{
			name: "single column",
			in: [][]int{
				{1}, {2}, {3},
			},
			want: [][]int{
				{1, 2, 3},
			},
		},
	}

	for _, tc := range cases {
		t.Run(tc.name, func(t *testing.T) {
			got := Transpose(tc.in)
			if len(got) != len(tc.want) {
				t.Fatalf("Transpose(%v) row count = %d, want %d", tc.in, len(got), len(tc.want))
			}
			for i := range got {
				if len(got[i]) != len(tc.want[i]) {
					t.Fatalf("Transpose(%v) row %d length = %d, want %d", tc.in, i, len(got[i]), len(tc.want[i]))
				}
			}
			if !reflect.DeepEqual(got, tc.want) {
				t.Errorf("Transpose(%v) = %v, want %v", tc.in, got, tc.want)
			}
		})
	}
}

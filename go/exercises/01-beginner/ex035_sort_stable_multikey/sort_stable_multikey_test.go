package sortstablemultikey

import "testing"

func TestSortByGradeThenName(t *testing.T) {
	cases := []struct {
		name  string
		input []Student
		want  []Student
	}{
		{
			name: "ties preserve original order",
			input: []Student{
				{Name: "Alice", Grade: 2},
				{Name: "Bob", Grade: 1},
				{Name: "Carol", Grade: 2},
				{Name: "Dave", Grade: 1},
				{Name: "Eve", Grade: 3},
			},
			want: []Student{
				{Name: "Bob", Grade: 1},
				{Name: "Dave", Grade: 1},
				{Name: "Alice", Grade: 2},
				{Name: "Carol", Grade: 2},
				{Name: "Eve", Grade: 3},
			},
		},
		{
			name: "already sorted, no ties",
			input: []Student{
				{Name: "Zoe", Grade: 1},
				{Name: "Amy", Grade: 2},
				{Name: "Tom", Grade: 3},
			},
			want: []Student{
				{Name: "Zoe", Grade: 1},
				{Name: "Amy", Grade: 2},
				{Name: "Tom", Grade: 3},
			},
		},
		{
			name: "all same grade keeps original order",
			input: []Student{
				{Name: "Gus", Grade: 5},
				{Name: "Fay", Grade: 5},
				{Name: "Hal", Grade: 5},
			},
			want: []Student{
				{Name: "Gus", Grade: 5},
				{Name: "Fay", Grade: 5},
				{Name: "Hal", Grade: 5},
			},
		},
		{
			name:  "empty slice",
			input: []Student{},
			want:  []Student{},
		},
	}

	for _, tc := range cases {
		t.Run(tc.name, func(t *testing.T) {
			SortByGradeThenName(tc.input)
			if len(tc.input) != len(tc.want) {
				t.Fatalf("length = %d, want %d", len(tc.input), len(tc.want))
			}
			for i := range tc.want {
				if tc.input[i] != tc.want[i] {
					t.Errorf("index %d = %+v, want %+v", i, tc.input[i], tc.want[i])
				}
			}
		})
	}
}

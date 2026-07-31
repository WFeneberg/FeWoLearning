package pointerlinkedlist

import "testing"

func TestListString(t *testing.T) {
	cases := []struct {
		name   string
		values []int
		want   string
	}{
		{"empty", []int{}, "[]"},
		{"single", []int{42}, "[42]"},
		{"multiple", []int{1, 2, 3}, "[1 2 3]"},
		{"insertion order preserved", []int{5, 3, 9, 1}, "[5 3 9 1]"},
	}

	for _, tc := range cases {
		t.Run(tc.name, func(t *testing.T) {
			var l List
			for _, v := range tc.values {
				l.Push(v)
			}
			if got := l.String(); got != tc.want {
				t.Errorf("String() = %q, want %q", got, tc.want)
			}
		})
	}
}

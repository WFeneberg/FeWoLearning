package stringbuilderjoin

import "testing"

func TestJoinWithBuilder(t *testing.T) {
	cases := []struct {
		parts []string
		sep   string
		want  string
	}{
		{[]string{"a", "b", "c"}, ",", "a,b,c"},
		{[]string{"hello", "world"}, " ", "hello world"},
		{[]string{"one"}, "-", "one"},
		{[]string{}, ",", ""},
		{[]string{"x", "y", "z"}, "", "xyz"},
	}
	for _, c := range cases {
		if got := JoinWithBuilder(c.parts, c.sep); got != c.want {
			t.Errorf("JoinWithBuilder(%v, %q) = %q, want %q", c.parts, c.sep, got, c.want)
		}
	}
}

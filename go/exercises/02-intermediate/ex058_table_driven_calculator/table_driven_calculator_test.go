package tabledrivencalculator

import "testing"

func TestCalculate(t *testing.T) {
	cases := []struct {
		name    string
		op      string
		a, b    float64
		want    float64
		wantErr bool
	}{
		{name: "add", op: "add", a: 2, b: 3, want: 5},
		{name: "sub", op: "sub", a: 5, b: 3, want: 2},
		{name: "mul", op: "mul", a: 4, b: 3, want: 12},
		{name: "div", op: "div", a: 9, b: 3, want: 3},
		{name: "div by zero", op: "div", a: 1, b: 0, wantErr: true},
		{name: "unknown op", op: "mod", a: 1, b: 2, wantErr: true},
	}

	for _, tc := range cases {
		t.Run(tc.name, func(t *testing.T) {
			got, err := Calculate(tc.op, tc.a, tc.b)
			if tc.wantErr {
				if err == nil {
					t.Fatalf("Calculate(%q, %v, %v) = %v, nil; want error", tc.op, tc.a, tc.b, got)
				}
				return
			}
			if err != nil {
				t.Fatalf("Calculate(%q, %v, %v) unexpected error: %v", tc.op, tc.a, tc.b, err)
			}
			if got != tc.want {
				t.Errorf("Calculate(%q, %v, %v) = %v, want %v", tc.op, tc.a, tc.b, got, tc.want)
			}
		})
	}
}

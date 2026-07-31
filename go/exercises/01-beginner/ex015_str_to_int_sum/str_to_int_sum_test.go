package strtointsum

import "testing"

func TestSumCSV(t *testing.T) {
	cases := []struct {
		name    string
		in      string
		want    int
		wantErr bool
	}{
		{"single", "5", 5, false},
		{"multiple", "1,2,3", 6, false},
		{"negatives", "-1,2,-3", -2, false},
		{"invalid token", "1,two,3", 0, true},
		{"trailing garbage", "1,2,3x", 0, true},
	}
	for _, tc := range cases {
		t.Run(tc.name, func(t *testing.T) {
			got, err := SumCSV(tc.in)
			if tc.wantErr {
				if err == nil {
					t.Fatalf("SumCSV(%q) error = nil, want non-nil", tc.in)
				}
				return
			}
			if err != nil {
				t.Fatalf("SumCSV(%q) unexpected error: %v", tc.in, err)
			}
			if got != tc.want {
				t.Errorf("SumCSV(%q) = %d, want %d", tc.in, got, tc.want)
			}
		})
	}
}

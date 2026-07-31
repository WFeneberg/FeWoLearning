package workerwaitgroup

import "testing"

func sequentialSum(chunks [][]int) int {
	total := 0
	for _, chunk := range chunks {
		for _, v := range chunk {
			total += v
		}
	}
	return total
}

func TestSumConcurrently(t *testing.T) {
	cases := []struct {
		name   string
		chunks [][]int
	}{
		{"empty", [][]int{}},
		{"single chunk", [][]int{{1, 2, 3, 4, 5}}},
		{"multiple chunks", [][]int{{1, 2, 3}, {4, 5}, {6, 7, 8, 9}}},
		{"chunk with negatives", [][]int{{-10, 20}, {5, -5}, {100}}},
		{"empty chunks mixed in", [][]int{{}, {1, 2}, {}, {3}}},
		{"many small chunks", [][]int{{1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}}},
	}

	for _, tc := range cases {
		t.Run(tc.name, func(t *testing.T) {
			want := sequentialSum(tc.chunks)
			got := SumConcurrently(tc.chunks)
			if got != want {
				t.Errorf("SumConcurrently(%v) = %d, want %d", tc.chunks, got, want)
			}
		})
	}
}

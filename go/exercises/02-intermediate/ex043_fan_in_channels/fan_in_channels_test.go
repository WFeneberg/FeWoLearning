package faninchannels

import (
	"sort"
	"testing"
	"time"
)

// makeChan returns a receive-only channel that emits vals and then closes.
func makeChan(vals ...int) <-chan int {
	ch := make(chan int, len(vals))
	for _, v := range vals {
		ch <- v
	}
	close(ch)
	return ch
}

func TestMerge(t *testing.T) {
	cases := []struct {
		name string
		in   [][]int
		want []int
	}{
		{
			name: "three sources",
			in:   [][]int{{1, 2, 3}, {4, 5}, {6}},
			want: []int{1, 2, 3, 4, 5, 6},
		},
		{
			name: "single source",
			in:   [][]int{{10, 20, 30}},
			want: []int{10, 20, 30},
		},
		{
			name: "empty sources",
			in:   [][]int{{}, {}, {}},
			want: []int{},
		},
		{
			name: "no sources",
			in:   [][]int{},
			want: []int{},
		},
		{
			name: "duplicates across sources",
			in:   [][]int{{1, 1}, {1}, {2}},
			want: []int{1, 1, 1, 2},
		},
	}

	for _, tc := range cases {
		tc := tc
		t.Run(tc.name, func(t *testing.T) {
			chans := make([]<-chan int, len(tc.in))
			for i, vals := range tc.in {
				chans[i] = makeChan(vals...)
			}

			out := Merge(chans...)

			got := []int{}
			timeout := time.After(2 * time.Second)
		loop:
			for {
				select {
				case v, ok := <-out:
					if !ok {
						break loop
					}
					got = append(got, v)
				case <-timeout:
					t.Fatal("Merge did not close output channel in time")
				}
			}

			sort.Ints(got)
			sort.Ints(tc.want)

			if len(got) != len(tc.want) {
				t.Fatalf("Merge(%v) = %v, want %v", tc.in, got, tc.want)
			}
			for i := range got {
				if got[i] != tc.want[i] {
					t.Fatalf("Merge(%v) = %v, want %v", tc.in, got, tc.want)
				}
			}
		})
	}
}

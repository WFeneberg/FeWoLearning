package workerpoolfixed

import (
	"reflect"
	"sync/atomic"
	"testing"
)

func TestRunPoolPreservesOrderAndTransforms(t *testing.T) {
	cases := []struct {
		name    string
		jobs    []int
		workers int
	}{
		{"fewer workers than jobs", []int{1, 2, 3, 4, 5, 6, 7, 8, 9, 10}, 3},
		{"more workers than jobs", []int{1, 2, 3}, 8},
		{"single worker", []int{5, 4, 3, 2, 1}, 1},
		{"single job", []int{42}, 4},
		{"no jobs", []int{}, 4},
	}

	square := func(n int) int { return n * n }

	for _, tc := range cases {
		t.Run(tc.name, func(t *testing.T) {
			want := make([]int, len(tc.jobs))
			for i, j := range tc.jobs {
				want[i] = square(j)
			}

			got := RunPool(tc.jobs, tc.workers, square)

			if !reflect.DeepEqual(got, want) {
				t.Fatalf("RunPool(%v, %d) = %v, want %v", tc.jobs, tc.workers, got, want)
			}
		})
	}
}

// TestRunPoolProcessesEachJobExactlyOnce guards against workers stealing or
// duplicating work: every index of the input must be visited exactly once,
// no matter how many workers race to consume the job channel.
func TestRunPoolProcessesEachJobExactlyOnce(t *testing.T) {
	const n = 200
	jobs := make([]int, n)
	for i := range jobs {
		jobs[i] = i
	}

	var hits [n]int32
	f := func(v int) int {
		atomic.AddInt32(&hits[v], 1)
		return v + 100
	}

	got := RunPool(jobs, 16, f)

	if len(got) != n {
		t.Fatalf("len(got) = %d, want %d", len(got), n)
	}
	for i, v := range got {
		if v != i+100 {
			t.Errorf("got[%d] = %d, want %d", i, v, i+100)
		}
		if c := atomic.LoadInt32(&hits[i]); c != 1 {
			t.Errorf("job %d processed %d times, want exactly 1", i, c)
		}
	}
}

func TestRunPoolPanicsOnNonPositiveWorkers(t *testing.T) {
	for _, workers := range []int{0, -1, -5} {
		func() {
			defer func() {
				if recover() == nil {
					t.Errorf("RunPool with workers=%d: expected panic, got none", workers)
				}
			}()
			RunPool([]int{1, 2, 3}, workers, func(n int) int { return n })
		}()
	}
}

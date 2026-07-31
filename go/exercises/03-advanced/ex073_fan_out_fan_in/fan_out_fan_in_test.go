package fanoutfanin

import "testing"

func square(n int) int { return n * n }

func double(n int) int { return n * 2 }

func TestProcess(t *testing.T) {
	cases := []struct {
		name    string
		inputs  []int
		workers int
		work    func(int) int
		want    int
	}{
		{"squares_single_worker", []int{1, 2, 3, 4, 5}, 1, square, 55},
		{"squares_more_workers_than_cpus", []int{1, 2, 3, 4, 5}, 8, square, 55},
		{"doubles_more_workers_than_inputs", []int{10, 20, 30}, 10, double, 120},
		{"empty_inputs", []int{}, 4, square, 0},
		{"single_input", []int{7}, 3, square, 49},
		{"negative_and_positive", []int{-3, -2, -1, 0, 1, 2, 3}, 4, square, 28},
		{"larger_batch", []int{1, 2, 3, 4, 5, 6, 7, 8, 9, 10}, 3, double, 110},
	}

	for _, tc := range cases {
		t.Run(tc.name, func(t *testing.T) {
			got := Process(tc.inputs, tc.workers, tc.work)
			if got != tc.want {
				t.Errorf("Process(%v, workers=%d) = %d, want %d", tc.inputs, tc.workers, got, tc.want)
			}
		})
	}
}

func TestProcessRepeatedRunsAreConsistent(t *testing.T) {
	inputs := []int{2, 4, 6, 8, 10, 12, 14}
	const want = 2 + 4 + 6 + 8 + 10 + 12 + 14
	for i := 0; i < 20; i++ {
		if got := Process(inputs, 5, func(n int) int { return n }); got != want {
			t.Fatalf("run %d: Process(...) = %d, want %d", i, got, want)
		}
	}
}

func TestProcessPanicsOnInvalidWorkerCount(t *testing.T) {
	for _, workers := range []int{0, -1} {
		func() {
			defer func() {
				if recover() == nil {
					t.Errorf("Process with workers=%d: expected panic, got none", workers)
				}
			}()
			Process([]int{1, 2, 3}, workers, square)
		}()
	}
}

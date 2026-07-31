// Package fanoutfanin — Exercise 073 (reference solution).
package fanoutfanin

import "sync"

func Process(inputs []int, workers int, work func(int) int) int {
	if workers <= 0 {
		panic("fanoutfanin: workers must be positive")
	}

	in := make(chan int)
	out := make(chan int)

	// Fan out: start a fixed pool of workers all reading from the same
	// input channel, so work is distributed across them as they become free.
	var wg sync.WaitGroup
	wg.Add(workers)
	for i := 0; i < workers; i++ {
		go func() {
			defer wg.Done()
			for n := range in {
				out <- work(n)
			}
		}()
	}

	// Feed the input channel from the caller's data on its own goroutine so
	// sends never deadlock against the workers draining it.
	go func() {
		defer close(in)
		for _, v := range inputs {
			in <- v
		}
	}()

	// Fan in: close out once every worker has finished, so the range below
	// terminates instead of blocking forever.
	go func() {
		wg.Wait()
		close(out)
	}()

	sum := 0
	for v := range out {
		sum += v
	}
	return sum
}

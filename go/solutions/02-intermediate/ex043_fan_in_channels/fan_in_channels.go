// Package faninchannels — Exercise 043 (reference solution).
package faninchannels

import "sync"

// Merge fans in values from all chans into a single returned channel.
// The returned channel is closed once every input channel has been
// drained and closed.
func Merge(chans ...<-chan int) <-chan int {
	out := make(chan int)
	var wg sync.WaitGroup
	wg.Add(len(chans))

	for _, c := range chans {
		go func(c <-chan int) {
			defer wg.Done()
			for v := range c {
				out <- v
			}
		}(c)
	}

	go func() {
		wg.Wait()
		close(out)
	}()

	return out
}

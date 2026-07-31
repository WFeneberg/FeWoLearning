package channelsemaphore

import (
	"sync"
	"sync/atomic"
	"testing"
)

// TestTryAcquire checks basic non-blocking acquire/release bookkeeping on a
// semaphore of capacity 1.
func TestTryAcquire(t *testing.T) {
	sem := NewSemaphore(1)

	if !sem.TryAcquire() {
		t.Fatalf("TryAcquire() on empty semaphore = false, want true")
	}
	if sem.TryAcquire() {
		t.Fatalf("TryAcquire() on full semaphore = true, want false")
	}

	sem.Release()

	if !sem.TryAcquire() {
		t.Fatalf("TryAcquire() after Release() = false, want true")
	}
}

// TestMaxConcurrentHolders drives many goroutines through the semaphore and
// verifies that the observed maximum number of simultaneous holders never
// exceeds the configured limit, and that it actually reaches the limit
// (i.e. the semaphore isn't overly conservative).
func TestMaxConcurrentHolders(t *testing.T) {
	const limit = 4
	const workers = limit * 3

	sem := NewSemaphore(limit)

	var cur int32 // number of goroutines currently holding the semaphore
	var max int32 // highest value cur has ever reached

	// ready is used as a rendezvous: each goroutine signals it has
	// acquired the semaphore and is now parked, waiting for the test to
	// release everyone at once. Because the channel has capacity `limit`
	// and the semaphore itself only allows `limit` concurrent holders,
	// draining exactly `limit` values from it deterministically proves
	// that many goroutines are holding the semaphore simultaneously,
	// with no sleeps or timing assumptions required.
	ready := make(chan struct{}, limit)
	proceed := make(chan struct{})

	var wg sync.WaitGroup
	for i := 0; i < workers; i++ {
		wg.Add(1)
		go func() {
			defer wg.Done()

			sem.Acquire()

			c := atomic.AddInt32(&cur, 1)
			for {
				m := atomic.LoadInt32(&max)
				if c <= m || atomic.CompareAndSwapInt32(&max, m, c) {
					break
				}
			}

			ready <- struct{}{}
			<-proceed

			atomic.AddInt32(&cur, -1)
			sem.Release()
		}()
	}

	// Wait until `limit` goroutines are simultaneously parked holding the
	// semaphore before letting any of them go.
	for i := 0; i < limit; i++ {
		<-ready
	}
	close(proceed)
	wg.Wait()

	if got := atomic.LoadInt32(&max); got != limit {
		t.Fatalf("observed max concurrent holders = %d, want exactly %d", got, limit)
	}
	if got := atomic.LoadInt32(&cur); got != 0 {
		t.Fatalf("current holders after all released = %d, want 0", got)
	}

	// The semaphore must be usable again afterwards.
	if !sem.TryAcquire() {
		t.Fatalf("TryAcquire() after all released = false, want true")
	}
}

// Package channelsemaphore — Exercise 068 (intermediate).
// Goal:   implement a bounded-concurrency Semaphore backed by a buffered
//         channel, with Acquire/Release methods that limit how many
//         goroutines may hold the semaphore at once.
// Drills: buffered channels as semaphores, goroutine coordination,
//         sync.WaitGroup, atomic counters.
package channelsemaphore

// Semaphore limits the number of concurrent holders to a fixed capacity.
type Semaphore struct {
	slots chan struct{}
}

// NewSemaphore returns a Semaphore that allows at most n concurrent holders.
// It panics if n <= 0.
func NewSemaphore(n int) *Semaphore {
	panic("TODO: implement NewSemaphore")
}

// Acquire blocks until a slot is available, then takes it.
func (s *Semaphore) Acquire() {
	panic("TODO: implement Acquire")
}

// Release returns a previously acquired slot to the semaphore.
func (s *Semaphore) Release() {
	panic("TODO: implement Release")
}

// TryAcquire attempts to acquire a slot without blocking. It reports
// whether the slot was acquired.
func (s *Semaphore) TryAcquire() bool {
	panic("TODO: implement TryAcquire")
}

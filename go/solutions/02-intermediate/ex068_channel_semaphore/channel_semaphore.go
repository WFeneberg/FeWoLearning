// Package channelsemaphore — Exercise 068 (reference solution).
package channelsemaphore

// Semaphore limits the number of concurrent holders to a fixed capacity.
type Semaphore struct {
	slots chan struct{}
}

// NewSemaphore returns a Semaphore that allows at most n concurrent holders.
// It panics if n <= 0.
func NewSemaphore(n int) *Semaphore {
	if n <= 0 {
		panic("channelsemaphore: capacity must be positive")
	}
	return &Semaphore{slots: make(chan struct{}, n)}
}

// Acquire blocks until a slot is available, then takes it.
func (s *Semaphore) Acquire() {
	s.slots <- struct{}{}
}

// Release returns a previously acquired slot to the semaphore.
func (s *Semaphore) Release() {
	<-s.slots
}

// TryAcquire attempts to acquire a slot without blocking. It reports
// whether the slot was acquired.
func (s *Semaphore) TryAcquire() bool {
	select {
	case s.slots <- struct{}{}:
		return true
	default:
		return false
	}
}

// Package synconcesingleton — Exercise 067 (intermediate).
// Goal:   Implement GetInstance() *Config that lazily initializes a single
//         shared *Config exactly once, even when many goroutines call it
//         concurrently, using sync.Once.
// Drills: sync.Once, lazy initialization, singleton pattern, concurrency safety.
package synconcesingleton

// Config represents an expensive-to-build shared resource.
type Config struct {
	Value int
}

// InitCount tracks how many times the initializer has actually run.
// It is exported so tests can assert the initializer ran exactly once.
var InitCount int

// GetInstance returns the singleton *Config, initializing it lazily on the
// first call. Concurrent callers must all observe the same instance, and the
// initialization logic must run exactly once regardless of how many
// goroutines call GetInstance concurrently. The initializer sets Value to 42
// and increments InitCount.
func GetInstance() *Config {
	panic("TODO: implement GetInstance using sync.Once")
}

// Package synconcesingleton — Exercise 067 (reference solution).
package synconcesingleton

import "sync"

// Config represents an expensive-to-build shared resource.
type Config struct {
	Value int
}

// InitCount tracks how many times the initializer has actually run.
var InitCount int

var (
	once     sync.Once
	instance *Config
)

// GetInstance returns the singleton *Config, initializing it lazily on the
// first call via sync.Once so the initializer runs exactly once even under
// concurrent access.
func GetInstance() *Config {
	once.Do(func() {
		InitCount++
		instance = &Config{Value: 42}
	})
	return instance
}

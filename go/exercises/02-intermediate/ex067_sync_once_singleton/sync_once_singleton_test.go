package synconcesingleton

import (
	"sync"
	"testing"
)

// sync.Once fires at most once per process, so InitCount can only ever be
// observed going 0 -> 1 by whichever test races there first. Asserting the
// pointer identity and the init count in one test keeps this independent of
// test order; a later test that reset InitCount could never make it 1 again,
// because the initializer would already have been consumed.
func TestGetInstanceIsSingletonInitializedExactlyOnce(t *testing.T) {
	const goroutines = 100

	instances := make([]*Config, goroutines)
	var wg sync.WaitGroup
	wg.Add(goroutines)

	for i := 0; i < goroutines; i++ {
		i := i
		go func() {
			defer wg.Done()
			instances[i] = GetInstance()
		}()
	}
	wg.Wait()

	first := instances[0]
	if first == nil {
		t.Fatal("GetInstance() returned nil")
	}

	for i, inst := range instances {
		if inst != first {
			t.Errorf("instance %d = %p, want same pointer as instance 0 = %p", i, inst, first)
		}
	}

	if InitCount != 1 {
		t.Errorf("InitCount = %d, want 1 (initializer must run exactly once)", InitCount)
	}
}

func TestGetInstanceReturnsUsableConfig(t *testing.T) {
	cfg := GetInstance()
	if cfg == nil {
		t.Fatal("GetInstance() returned nil")
	}
	// Value should be a stable, deterministic constant set by the initializer.
	if cfg.Value != 42 {
		t.Errorf("cfg.Value = %d, want 42", cfg.Value)
	}
}

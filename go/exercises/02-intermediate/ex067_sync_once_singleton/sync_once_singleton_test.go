package synconcesingleton

import (
	"sync"
	"testing"
)

func TestGetInstanceSameAcrossConcurrentCalls(t *testing.T) {
	const goroutines = 100

	InitCount = 0

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
}

func TestGetInstanceInitializesExactlyOnce(t *testing.T) {
	const goroutines = 50

	InitCount = 0

	var wg sync.WaitGroup
	wg.Add(goroutines)
	for i := 0; i < goroutines; i++ {
		go func() {
			defer wg.Done()
			GetInstance()
		}()
	}
	wg.Wait()

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

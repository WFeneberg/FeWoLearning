package goroutinecountermutex

import (
	"sync"
	"testing"
)

func TestSafeCounterConcurrentIncrement(t *testing.T) {
	cases := []struct {
		name        string
		goroutines  int
		incsPerGoro int
		want        int
	}{
		{"small", 10, 100, 1000},
		{"medium", 50, 200, 10000},
		{"many_goroutines_few_incs", 500, 10, 5000},
	}

	for _, tc := range cases {
		tc := tc
		t.Run(tc.name, func(t *testing.T) {
			c := &SafeCounter{}
			var wg sync.WaitGroup
			wg.Add(tc.goroutines)
			for i := 0; i < tc.goroutines; i++ {
				go func() {
					defer wg.Done()
					for j := 0; j < tc.incsPerGoro; j++ {
						c.Increment()
					}
				}()
			}
			wg.Wait()

			if got := c.Value(); got != tc.want {
				t.Errorf("Value() = %d, want %d", got, tc.want)
			}
		})
	}
}

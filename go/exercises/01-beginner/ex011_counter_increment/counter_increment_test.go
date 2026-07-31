package counterincrement

import "testing"

func TestCounterIncrement(t *testing.T) {
	cases := map[int]int{
		1: 1, 3: 3, 5: 5, 10: 10,
	}
	for calls, want := range cases {
		c := &Counter{}
		for i := 0; i < calls; i++ {
			c.Increment()
		}
		if got := c.Value(); got != want {
			t.Errorf("after %d Increment calls, Value() = %d, want %d", calls, got, want)
		}
	}
}

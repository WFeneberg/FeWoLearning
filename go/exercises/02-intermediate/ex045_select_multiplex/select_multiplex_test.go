package selectmultiplex

import "testing"

func TestCountFromBoth(t *testing.T) {
	a := make(chan int, 3)
	b := make(chan int, 2)

	a <- 1
	a <- 2
	a <- 3
	close(a)

	b <- 10
	b <- 20
	close(b)

	got := CountFromBoth(a, b, 5)

	if got["a"] != 3 {
		t.Errorf("counts[\"a\"] = %d, want 3", got["a"])
	}
	if got["b"] != 2 {
		t.Errorf("counts[\"b\"] = %d, want 2", got["b"])
	}
	if sum := got["a"] + got["b"]; sum != 5 {
		t.Errorf("total received = %d, want 5", sum)
	}
}

func TestCountFromBothPartial(t *testing.T) {
	a := make(chan int, 5)
	b := make(chan int, 5)

	for i := 0; i < 4; i++ {
		a <- i
	}
	for i := 0; i < 4; i++ {
		b <- i
	}
	close(a)
	close(b)

	got := CountFromBoth(a, b, 6)

	if sum := got["a"] + got["b"]; sum != 6 {
		t.Errorf("total received = %d, want 6", sum)
	}
	if got["a"] > 4 || got["b"] > 4 {
		t.Errorf("counts[\"a\"]=%d counts[\"b\"]=%d exceed available items", got["a"], got["b"])
	}
}

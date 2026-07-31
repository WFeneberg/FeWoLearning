package fizzbuzz

import "testing"

func TestEvaluate(t *testing.T) {
	cases := map[int]string{
		1: "1", 2: "2", 3: "Fizz", 5: "Buzz", 7: "7", 15: "FizzBuzz", 30: "FizzBuzz",
	}
	for n, want := range cases {
		if got := Evaluate(n); got != want {
			t.Errorf("Evaluate(%d) = %q, want %q", n, got, want)
		}
	}
}

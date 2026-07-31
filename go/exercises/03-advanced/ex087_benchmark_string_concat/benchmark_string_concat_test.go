package benchmarkstringconcat

import "testing"

func TestConcatStrategiesProduceSameResult(t *testing.T) {
	cases := []struct {
		name  string
		words []string
		want  string
	}{
		{"empty", []string{}, ""},
		{"single", []string{"go"}, "go"},
		{"several", []string{"go", "is", "fun"}, "goisfun"},
		{"repeated", []string{"a", "a", "a", "a"}, "aaaa"},
	}

	for _, tc := range cases {
		t.Run(tc.name, func(t *testing.T) {
			if got := ConcatPlus(tc.words); got != tc.want {
				t.Errorf("ConcatPlus(%v) = %q, want %q", tc.words, got, tc.want)
			}
			if got := ConcatBuilder(tc.words); got != tc.want {
				t.Errorf("ConcatBuilder(%v) = %q, want %q", tc.words, got, tc.want)
			}
		})
	}
}

func TestConcatStrategiesAgreeOnLargerInput(t *testing.T) {
	words := benchWords()
	plus := ConcatPlus(words)
	builder := ConcatBuilder(words)
	if plus != builder {
		t.Fatalf("ConcatPlus and ConcatBuilder disagree: len(plus)=%d len(builder)=%d", len(plus), len(builder))
	}

	want := ""
	for _, w := range words {
		want += w
	}
	if plus != want {
		t.Errorf("result mismatch: got len %d, want len %d", len(plus), len(want))
	}
}

// TestBenchmarksRun exercises the Benchmark* functions directly (as
// `go test -bench` would) to make sure they run to completion without
// panicking, independent of the timing numbers they produce.
func TestBenchmarksRun(t *testing.T) {
	for name, fn := range map[string]func(*testing.B){
		"ConcatPlus":    BenchmarkConcatPlus,
		"ConcatBuilder": BenchmarkConcatBuilder,
	} {
		t.Run(name, func(t *testing.T) {
			result := testing.Benchmark(fn)
			if result.N <= 0 {
				t.Errorf("benchmark %s ran N=%d iterations, want > 0", name, result.N)
			}
		})
	}
}

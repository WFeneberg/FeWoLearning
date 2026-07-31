package streaminglineprocessor

import (
	"bytes"
	"errors"
	"fmt"
	"strconv"
	"strings"
	"sync"
	"testing"
)

// TestProcessStream is table-driven over small, deterministic transforms and
// input shapes, checking the exact transformed output.
func TestProcessStream(t *testing.T) {
	cases := []struct {
		name      string
		input     string
		transform func(string) string
		want      string
	}{
		{
			name:      "empty input produces empty output",
			input:     "",
			transform: strings.ToUpper,
			want:      "",
		},
		{
			name:      "single line uppercased",
			input:     "hello",
			transform: strings.ToUpper,
			want:      "HELLO\n",
		},
		{
			name:      "multiple lines identity",
			input:     "one\ntwo\nthree",
			transform: func(s string) string { return s },
			want:      "one\ntwo\nthree\n",
		},
		{
			name:      "multiple lines uppercased preserves order",
			input:     "alpha\nbeta\ngamma\ndelta",
			transform: strings.ToUpper,
			want:      "ALPHA\nBETA\nGAMMA\nDELTA\n",
		},
		{
			name:  "transform can change length per line",
			input: "a\nbb\nccc",
			transform: func(s string) string {
				return fmt.Sprintf("%d:%s", len(s), s)
			},
			want: "1:a\n2:bb\n3:ccc\n",
		},
	}

	for _, tc := range cases {
		t.Run(tc.name, func(t *testing.T) {
			var out bytes.Buffer
			if err := ProcessStream(strings.NewReader(tc.input), tc.transform, &out); err != nil {
				t.Fatalf("ProcessStream returned error: %v", err)
			}
			if got := out.String(); got != tc.want {
				t.Errorf("output = %q, want %q", got, tc.want)
			}
		})
	}
}

// slowWriter simulates a writer that is much slower than the producer side
// of the pipeline by burning CPU on every call (deterministic, no wall-clock
// or randomness involved) before recording the write. It also records every
// chunk it receives so the test can verify strict ordering.
type slowWriter struct {
	mu     sync.Mutex
	writes []string
}

func (w *slowWriter) Write(p []byte) (int, error) {
	// Deterministic busy-work to emulate a slow consumer without relying on
	// time.Sleep or the wall clock.
	sum := 0
	for i := 0; i < 20000; i++ {
		sum += i * i
	}
	_ = sum

	w.mu.Lock()
	defer w.mu.Unlock()
	w.writes = append(w.writes, string(p))
	return len(p), nil
}

// TestProcessStreamOrderPreservedUnderSlowWriter drives many lines through
// the pipeline with an artificially slow writer and checks that every line
// still arrives, transformed, in the exact original order: the bounded
// channel must apply backpressure to the reader without ever reordering or
// dropping data.
func TestProcessStreamOrderPreservedUnderSlowWriter(t *testing.T) {
	const n = 200
	var inputLines []string
	var wantLines []string
	for i := 0; i < n; i++ {
		line := "line-" + strconv.Itoa(i)
		inputLines = append(inputLines, line)
		wantLines = append(wantLines, strings.ToUpper(line))
	}
	input := strings.Join(inputLines, "\n")

	sw := &slowWriter{}
	err := ProcessStream(strings.NewReader(input), strings.ToUpper, sw)
	if err != nil {
		t.Fatalf("ProcessStream returned error: %v", err)
	}

	got := strings.Join(sw.writes, "")
	want := strings.Join(wantLines, "\n") + "\n"
	if got != want {
		t.Fatalf("output mismatch under slow writer:\n got  = %q\n want = %q", got, want)
	}
}

// errBoom is a sentinel used to check that ProcessStream propagates the
// exact underlying error, not a wrapped/generic one that loses identity.
var errBoom = errors.New("boom")

// failingWriter returns errBoom once more than limit writes have occurred.
type failingWriter struct {
	mu    sync.Mutex
	count int
	limit int
}

func (w *failingWriter) Write(p []byte) (int, error) {
	w.mu.Lock()
	defer w.mu.Unlock()
	w.count++
	if w.count > w.limit {
		return 0, errBoom
	}
	return len(p), nil
}

func TestProcessStreamPropagatesWriterError(t *testing.T) {
	input := strings.Join([]string{"a", "b", "c", "d", "e", "f"}, "\n")
	fw := &failingWriter{limit: 2}

	err := ProcessStream(strings.NewReader(input), strings.ToUpper, fw)
	if !errors.Is(err, errBoom) {
		t.Fatalf("ProcessStream error = %v, want %v", err, errBoom)
	}
}

// errReader yields data then fails with errBoom instead of io.EOF, so the
// test can check that read-side errors are propagated too.
type errReader struct {
	data []byte
	pos  int
}

func (r *errReader) Read(p []byte) (int, error) {
	if r.pos >= len(r.data) {
		return 0, errBoom
	}
	n := copy(p, r.data[r.pos:])
	r.pos += n
	return n, nil
}

func TestProcessStreamPropagatesReadError(t *testing.T) {
	er := &errReader{data: []byte("x\ny\nz\n")}
	var out bytes.Buffer

	err := ProcessStream(er, strings.ToUpper, &out)
	if !errors.Is(err, errBoom) {
		t.Fatalf("ProcessStream error = %v, want %v", err, errBoom)
	}
}

// TestProcessStreamDoesNotDeadlockOnLargeInput exercises the pipeline with a
// volume of input several times larger than any reasonable internal buffer,
// combined with a slow writer, to make sure backpressure never causes a
// deadlock or a lost line.
func TestProcessStreamDoesNotDeadlockOnLargeInput(t *testing.T) {
	const n = 2000
	lines := make([]string, n)
	for i := range lines {
		lines[i] = strconv.Itoa(i)
	}
	input := strings.Join(lines, "\n")

	sw := &slowWriter{}
	done := make(chan error, 1)
	go func() {
		done <- ProcessStream(strings.NewReader(input), func(s string) string { return s }, sw)
	}()

	err := <-done
	if err != nil {
		t.Fatalf("ProcessStream returned error: %v", err)
	}

	got := strings.Join(sw.writes, "")
	want := strings.Join(lines, "\n") + "\n"
	if got != want {
		t.Fatalf("large input mismatch: got %d bytes, want %d bytes", len(got), len(want))
	}
}

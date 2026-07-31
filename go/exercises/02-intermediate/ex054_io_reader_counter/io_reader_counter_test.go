package ioreadercounter

import (
	"bytes"
	"io"
	"strings"
	"testing"
)

func TestCountingReader(t *testing.T) {
	cases := []struct {
		name string
		data string
	}{
		{"empty", ""},
		{"short", "hello"},
		{"longer", strings.Repeat("go-exercise-data-", 37)},
	}

	for _, tc := range cases {
		t.Run(tc.name, func(t *testing.T) {
			src := strings.NewReader(tc.data)
			cr := NewCountingReader(src)

			var dst bytes.Buffer
			n, err := io.Copy(&dst, cr)
			if err != nil {
				t.Fatalf("io.Copy returned error: %v", err)
			}

			if cr.Count != n {
				t.Errorf("Count = %d, want %d (io.Copy result)", cr.Count, n)
			}
			if cr.Count != int64(len(tc.data)) {
				t.Errorf("Count = %d, want %d (len of source data)", cr.Count, len(tc.data))
			}
			if dst.String() != tc.data {
				t.Errorf("copied data = %q, want %q", dst.String(), tc.data)
			}
		})
	}
}

func TestCountingReaderMultipleReads(t *testing.T) {
	src := strings.NewReader("abcdefghij")
	cr := NewCountingReader(src)

	buf := make([]byte, 3)
	total := 0
	for {
		n, err := cr.Read(buf)
		total += n
		if err == io.EOF {
			break
		}
		if err != nil {
			t.Fatalf("Read returned error: %v", err)
		}
	}

	if cr.Count != int64(total) {
		t.Errorf("Count = %d, want %d", cr.Count, total)
	}
	if cr.Count != 10 {
		t.Errorf("Count = %d, want 10", cr.Count)
	}
}

package interfaceembeddingreadwriter

import (
	"io"
	"testing"
)

func TestBufferSatisfiesReadWriter(t *testing.T) {
	var _ ReadWriter = NewBuffer()
}

func TestBufferRoundTrip(t *testing.T) {
	b := NewBuffer()

	n, err := b.Write([]byte("hello"))
	if err != nil {
		t.Fatalf("Write returned error: %v", err)
	}
	if n != 5 {
		t.Errorf("Write returned n = %d, want 5", n)
	}

	n2, err := b.Write([]byte(" world"))
	if err != nil {
		t.Fatalf("second Write returned error: %v", err)
	}
	if n2 != 6 {
		t.Errorf("second Write returned n = %d, want 6", n2)
	}

	out := make([]byte, 11)
	total := 0
	for total < 11 {
		n, err := b.Read(out[total:])
		total += n
		if err != nil {
			if err == io.EOF && total == 11 {
				break
			}
			t.Fatalf("Read returned error before consuming all data: %v (total=%d)", err, total)
		}
		if n == 0 {
			t.Fatalf("Read returned 0 bytes with nil error, would loop forever")
		}
	}

	if got, want := string(out[:total]), "hello world"; got != want {
		t.Errorf("round-tripped data = %q, want %q", got, want)
	}

	// Reading again should now report EOF since all bytes are consumed.
	extra := make([]byte, 4)
	n3, err := b.Read(extra)
	if err != io.EOF {
		t.Errorf("Read after exhaustion: err = %v, want io.EOF", err)
	}
	if n3 != 0 {
		t.Errorf("Read after exhaustion: n = %d, want 0", n3)
	}
}

func TestUseReadWriter(t *testing.T) {
	b := NewBuffer()
	got := UseReadWriter(b, "interface embedding")
	want := "interface embedding"
	if got != want {
		t.Errorf("UseReadWriter = %q, want %q", got, want)
	}
}

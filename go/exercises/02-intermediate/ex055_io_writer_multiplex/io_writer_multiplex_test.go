package iowritermultiplex

import (
	"bytes"
	"errors"
	"io"
	"testing"
)

func TestMultiplexWriter_Write(t *testing.T) {
	var a, b, c bytes.Buffer
	mw := NewMultiplexWriter(&a, &b, &c)

	data := []byte("hello, multiplex")
	n, err := mw.Write(data)
	if err != nil {
		t.Fatalf("Write returned unexpected error: %v", err)
	}
	if n != len(data) {
		t.Fatalf("Write returned n = %d, want %d", n, len(data))
	}

	for name, buf := range map[string]*bytes.Buffer{"a": &a, "b": &b, "c": &c} {
		if got := buf.String(); got != string(data) {
			t.Errorf("buffer %s = %q, want %q", name, got, string(data))
		}
	}

	// A second Write should append to all destinations identically.
	more := []byte(" again")
	if _, err := mw.Write(more); err != nil {
		t.Fatalf("second Write returned unexpected error: %v", err)
	}
	want := string(data) + string(more)
	for name, buf := range map[string]*bytes.Buffer{"a": &a, "b": &b, "c": &c} {
		if got := buf.String(); got != want {
			t.Errorf("after second write, buffer %s = %q, want %q", name, got, want)
		}
	}
}

func TestMultiplexWriter_NoWriters(t *testing.T) {
	mw := NewMultiplexWriter()
	n, err := mw.Write([]byte("anything"))
	if err != nil {
		t.Fatalf("Write with no destinations returned error: %v", err)
	}
	if n != len("anything") {
		t.Fatalf("Write with no destinations returned n = %d, want %d", n, len("anything"))
	}
}

type errWriter struct {
	err error
}

func (w errWriter) Write(p []byte) (int, error) {
	return 0, w.err
}

func TestMultiplexWriter_PropagatesError(t *testing.T) {
	var ok bytes.Buffer
	boom := errors.New("boom")
	mw := NewMultiplexWriter(&ok, errWriter{err: boom})

	_, err := mw.Write([]byte("x"))
	if !errors.Is(err, boom) {
		t.Fatalf("Write error = %v, want %v", err, boom)
	}
}

type shortWriter struct{}

func (shortWriter) Write(p []byte) (int, error) {
	if len(p) == 0 {
		return 0, nil
	}
	return len(p) - 1, nil
}

func TestMultiplexWriter_ShortWrite(t *testing.T) {
	var ok bytes.Buffer
	mw := NewMultiplexWriter(&ok, shortWriter{})

	_, err := mw.Write([]byte("hello"))
	if !errors.Is(err, io.ErrShortWrite) {
		t.Fatalf("Write error = %v, want %v", err, io.ErrShortWrite)
	}
}

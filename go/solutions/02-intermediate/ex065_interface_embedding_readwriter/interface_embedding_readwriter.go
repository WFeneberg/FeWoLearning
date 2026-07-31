// Package interfaceembeddingreadwriter — Exercise 065 (reference solution).
package interfaceembeddingreadwriter

import "io"

// ReadWriter combines io.Reader and io.Writer into a single interface via
// embedding. Any type satisfying both Read and Write automatically
// satisfies ReadWriter.
type ReadWriter interface {
	io.Reader
	io.Writer
}

// Buffer is a simple in-memory byte buffer that satisfies ReadWriter.
type Buffer struct {
	data []byte
	pos  int
}

// NewBuffer returns a new, empty *Buffer.
func NewBuffer() *Buffer {
	return &Buffer{}
}

// Write appends p to the buffer's storage and returns len(p), nil.
func (b *Buffer) Write(p []byte) (n int, err error) {
	b.data = append(b.data, p...)
	return len(p), nil
}

// Read copies unread bytes from the buffer into p, advancing the read
// position. It returns io.EOF once all written bytes have been consumed.
func (b *Buffer) Read(p []byte) (n int, err error) {
	if b.pos >= len(b.data) {
		return 0, io.EOF
	}
	n = copy(p, b.data[b.pos:])
	b.pos += n
	return n, nil
}

// UseReadWriter accepts any ReadWriter, writes msg to it, then reads all
// remaining bytes back out and returns them as a string.
func UseReadWriter(rw ReadWriter, msg string) string {
	if _, err := rw.Write([]byte(msg)); err != nil {
		panic(err)
	}
	out, err := io.ReadAll(rw)
	if err != nil {
		panic(err)
	}
	return string(out)
}

// Package interfaceembeddingreadwriter — Exercise 065 (intermediate).
// Goal:   Define a ReadWriter interface embedding io.Reader and io.Writer,
//         and a concrete Buffer type that satisfies it by round-tripping
//         written bytes back out on Read.
// Drills: interface embedding, io.Reader/io.Writer semantics, byte slices.
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
// Write appends bytes to the end of the internal storage, and Read
// consumes bytes from the front (FIFO order), advancing an internal
// read position.
type Buffer struct {
	data []byte
	pos  int
}

// NewBuffer returns a new, empty *Buffer.
func NewBuffer() *Buffer {
	panic("TODO: implement NewBuffer")
}

// Write appends p to the buffer's storage and returns len(p), nil.
func (b *Buffer) Write(p []byte) (n int, err error) {
	panic("TODO: implement Write")
}

// Read copies unread bytes from the buffer into p, advancing the read
// position. It returns io.EOF once all written bytes have been consumed.
func (b *Buffer) Read(p []byte) (n int, err error) {
	panic("TODO: implement Read")
}

// UseReadWriter accepts any ReadWriter, writes msg to it, then reads all
// remaining bytes back out and returns them as a string. It exists to
// prove that Buffer can be used wherever a ReadWriter is required.
func UseReadWriter(rw ReadWriter, msg string) string {
	panic("TODO: implement UseReadWriter")
}

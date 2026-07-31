// Package genericstack — Exercise 048 (intermediate).
// Goal:   Implement a generic Stack[T any] type with Push/Pop/Peek/Len/IsEmpty,
//         supporting any element type via Go generics.
// Drills: generics, type parameters, LIFO data structures.
package genericstack

// Stack is a generic LIFO stack holding elements of type T.
type Stack[T any] struct {
	items []T
}

// NewStack creates an empty Stack for elements of type T.
func NewStack[T any]() *Stack[T] {
	panic("TODO: implement NewStack")
}

// Push adds v to the top of the stack.
func (s *Stack[T]) Push(v T) {
	panic("TODO: implement Push")
}

// Pop removes and returns the top element of the stack.
// The second return value is false if the stack was empty.
func (s *Stack[T]) Pop() (T, bool) {
	panic("TODO: implement Pop")
}

// Peek returns the top element without removing it.
// The second return value is false if the stack is empty.
func (s *Stack[T]) Peek() (T, bool) {
	panic("TODO: implement Peek")
}

// Len returns the number of elements currently on the stack.
func (s *Stack[T]) Len() int {
	panic("TODO: implement Len")
}

// IsEmpty reports whether the stack has no elements.
func (s *Stack[T]) IsEmpty() bool {
	panic("TODO: implement IsEmpty")
}

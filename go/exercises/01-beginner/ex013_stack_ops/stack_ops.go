// Package stackops — Exercise 013 (beginner).
// Goal:   Implement a Stack type with Push and Pop() (int, error) methods,
//         where Pop on an empty stack returns a sentinel error.
// Drills: struct, custom errors, methods, LIFO ordering.
package stackops

import "errors"

// ErrEmptyStack is returned by Pop when the stack has no elements.
var ErrEmptyStack = errors.New("stack: empty stack")

// Stack is a simple LIFO stack of ints.
type Stack struct {
	items []int
}

// Push adds n to the top of the stack.
func (s *Stack) Push(n int) {
	panic("TODO: implement Push")
}

// Pop removes and returns the top item of the stack.
// If the stack is empty, it returns ErrEmptyStack.
func (s *Stack) Pop() (int, error) {
	panic("TODO: implement Pop")
}

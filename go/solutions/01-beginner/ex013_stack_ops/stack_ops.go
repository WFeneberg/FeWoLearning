// Package stackops — Exercise 013 (reference solution).
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
	s.items = append(s.items, n)
}

// Pop removes and returns the top item of the stack.
// If the stack is empty, it returns ErrEmptyStack.
func (s *Stack) Pop() (int, error) {
	if len(s.items) == 0 {
		return 0, ErrEmptyStack
	}
	last := len(s.items) - 1
	n := s.items[last]
	s.items = s.items[:last]
	return n, nil
}

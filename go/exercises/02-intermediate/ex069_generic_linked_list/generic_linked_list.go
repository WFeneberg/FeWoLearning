// Package genericlinkedlist — Exercise 069 (intermediate).
// Goal:   Implement a generic singly linked list LinkedList[T any] with
//         Append and ToSlice methods.
// Drills: generics, type parameters, linked data structures.
package genericlinkedlist

// node is a single element of the linked list.
type node[T any] struct {
	value T
	next  *node[T]
}

// LinkedList is a generic singly linked list.
type LinkedList[T any] struct {
	head *node[T]
	tail *node[T]
	size int
}

// Append adds a value to the end of the list.
func (l *LinkedList[T]) Append(v T) {
	panic("TODO: implement Append")
}

// ToSlice returns the list contents as a slice in order.
func (l *LinkedList[T]) ToSlice() []T {
	panic("TODO: implement ToSlice")
}

// Len returns the number of elements in the list.
func (l *LinkedList[T]) Len() int {
	panic("TODO: implement Len")
}

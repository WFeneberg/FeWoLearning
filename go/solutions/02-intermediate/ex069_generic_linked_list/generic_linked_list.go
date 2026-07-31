// Package genericlinkedlist — Exercise 069 (reference solution).
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
	n := &node[T]{value: v}
	if l.tail == nil {
		l.head = n
		l.tail = n
	} else {
		l.tail.next = n
		l.tail = n
	}
	l.size++
}

// ToSlice returns the list contents as a slice in order.
func (l *LinkedList[T]) ToSlice() []T {
	out := make([]T, 0, l.size)
	for n := l.head; n != nil; n = n.next {
		out = append(out, n.value)
	}
	return out
}

// Len returns the number of elements in the list.
func (l *LinkedList[T]) Len() int {
	return l.size
}

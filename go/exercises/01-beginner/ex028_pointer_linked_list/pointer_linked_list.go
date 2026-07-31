// Package pointerlinkedlist — Exercise 028 (beginner).
// Goal:   implement a singly linked List with Push(v int) and String(),
//         returning values in insertion order.
// Drills: pointers, structs, linked lists.
package pointerlinkedlist

// node is a single element of the linked list.
type node struct {
	val  int
	next *node
}

// List is a singly linked list of ints.
type List struct {
	head *node
	tail *node
}

// Push appends v to the end of the list.
func (l *List) Push(v int) {
	panic("TODO: implement Push")
}

// String returns the list values in insertion order, e.g. "[1 2 3]".
func (l *List) String() string {
	panic("TODO: implement String")
}

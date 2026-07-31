// Package pointerlinkedlist — Exercise 028 (reference solution).
package pointerlinkedlist

import (
	"strconv"
	"strings"
)

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
	n := &node{val: v}
	if l.head == nil {
		l.head = n
		l.tail = n
		return
	}
	l.tail.next = n
	l.tail = n
}

// String returns the list values in insertion order, e.g. "[1 2 3]".
func (l *List) String() string {
	var parts []string
	for n := l.head; n != nil; n = n.next {
		parts = append(parts, strconv.Itoa(n.val))
	}
	return "[" + strings.Join(parts, " ") + "]"
}

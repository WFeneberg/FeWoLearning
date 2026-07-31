// Package heappriorityqueue — Exercise 083 (advanced).
// Goal:   Implement a priority queue of Task items on top of container/heap,
//         where Pop always yields the item with the numerically lowest
//         Priority first, and ties are broken by insertion order (FIFO).
// Drills: container/heap.Interface, heap.Push/heap.Pop/heap.Fix, indices.
package heappriorityqueue

import "container/heap"

// Task is a unit of work with a priority. Lower Priority values are more
// urgent and are popped first. seq records insertion order to break ties,
// and index tracks the item's current position in the heap so Update can
// call heap.Fix efficiently.
type Task struct {
	ID       string
	Priority int

	seq   int
	index int
}

// PriorityQueue is a min-heap of *Task ordered by Priority (ties by seq).
// It implements container/heap.Interface.
type PriorityQueue struct {
	items []*Task
	seq   int
}

// NewPriorityQueue returns an empty, ready-to-use priority queue.
func NewPriorityQueue() *PriorityQueue {
	panic("TODO: implement NewPriorityQueue")
}

// Len returns the number of tasks currently queued.
func (pq *PriorityQueue) Len() int {
	panic("TODO: implement Len")
}

// Less reports whether the task at index i should be popped before j.
func (pq *PriorityQueue) Less(i, j int) bool {
	panic("TODO: implement Less")
}

// Swap exchanges the tasks at indices i and j and keeps their index fields
// in sync, as required by container/heap.Interface.
func (pq *PriorityQueue) Swap(i, j int) {
	panic("TODO: implement Swap")
}

// Push appends x (a *Task) to the heap's backing storage. Callers should use
// heap.Push (or PushTask) rather than calling this directly.
func (pq *PriorityQueue) Push(x any) {
	panic("TODO: implement Push")
}

// Pop removes and returns the last element of the backing storage. Callers
// should use heap.Pop (or PopTask) rather than calling this directly.
func (pq *PriorityQueue) Pop() any {
	panic("TODO: implement Pop")
}

// PushTask adds a task with the given id and priority to the queue,
// restoring the heap invariant.
func (pq *PriorityQueue) PushTask(id string, priority int) *Task {
	panic("TODO: implement PushTask")
}

// PopTask removes and returns the highest-priority task (lowest Priority
// value, ties broken by insertion order). It panics if the queue is empty.
func (pq *PriorityQueue) PopTask() *Task {
	panic("TODO: implement PopTask")
}

// Update changes an existing task's priority and restores the heap
// invariant in O(log n).
func (pq *PriorityQueue) Update(task *Task, priority int) {
	panic("TODO: implement Update")
}

var _ heap.Interface = (*PriorityQueue)(nil)

// Package heappriorityqueue — Exercise 083 (reference solution).
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
	pq := &PriorityQueue{}
	heap.Init(pq)
	return pq
}

// Len returns the number of tasks currently queued.
func (pq *PriorityQueue) Len() int { return len(pq.items) }

// Less reports whether the task at index i should be popped before j.
func (pq *PriorityQueue) Less(i, j int) bool {
	a, b := pq.items[i], pq.items[j]
	if a.Priority != b.Priority {
		return a.Priority < b.Priority
	}
	return a.seq < b.seq
}

// Swap exchanges the tasks at indices i and j and keeps their index fields
// in sync, as required by container/heap.Interface.
func (pq *PriorityQueue) Swap(i, j int) {
	pq.items[i], pq.items[j] = pq.items[j], pq.items[i]
	pq.items[i].index = i
	pq.items[j].index = j
}

// Push appends x (a *Task) to the heap's backing storage. Callers should use
// heap.Push (or PushTask) rather than calling this directly.
func (pq *PriorityQueue) Push(x any) {
	task := x.(*Task)
	task.index = len(pq.items)
	pq.items = append(pq.items, task)
}

// Pop removes and returns the last element of the backing storage. Callers
// should use heap.Pop (or PopTask) rather than calling this directly.
func (pq *PriorityQueue) Pop() any {
	old := pq.items
	n := len(old)
	task := old[n-1]
	old[n-1] = nil // avoid memory leak
	task.index = -1
	pq.items = old[:n-1]
	return task
}

// PushTask adds a task with the given id and priority to the queue,
// restoring the heap invariant.
func (pq *PriorityQueue) PushTask(id string, priority int) *Task {
	task := &Task{ID: id, Priority: priority, seq: pq.seq}
	pq.seq++
	heap.Push(pq, task)
	return task
}

// PopTask removes and returns the highest-priority task (lowest Priority
// value, ties broken by insertion order). It panics if the queue is empty.
func (pq *PriorityQueue) PopTask() *Task {
	if pq.Len() == 0 {
		panic("heappriorityqueue: Pop on empty queue")
	}
	return heap.Pop(pq).(*Task)
}

// Update changes an existing task's priority and restores the heap
// invariant in O(log n).
func (pq *PriorityQueue) Update(task *Task, priority int) {
	task.Priority = priority
	heap.Fix(pq, task.index)
}

var _ heap.Interface = (*PriorityQueue)(nil)

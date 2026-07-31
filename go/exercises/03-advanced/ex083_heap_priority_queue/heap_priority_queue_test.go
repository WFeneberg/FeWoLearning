package heappriorityqueue

import "testing"

func TestPopOrderIgnoresPushOrder(t *testing.T) {
	pq := NewPriorityQueue()

	// Push in a deliberately scrambled order; Pop must still return tasks in
	// strict ascending priority order.
	pushOrder := []struct {
		id       string
		priority int
	}{
		{"deploy", 5},
		{"page-oncall", 1},
		{"cleanup", 9},
		{"restart-service", 2},
		{"rotate-logs", 7},
		{"security-patch", 1}, // ties with page-oncall; must come out after it (FIFO)
	}
	for _, p := range pushOrder {
		pq.PushTask(p.id, p.priority)
	}

	if got := pq.Len(); got != len(pushOrder) {
		t.Fatalf("Len() = %d, want %d", got, len(pushOrder))
	}

	wantOrder := []struct {
		id       string
		priority int
	}{
		{"page-oncall", 1},
		{"security-patch", 1},
		{"restart-service", 2},
		{"deploy", 5},
		{"rotate-logs", 7},
		{"cleanup", 9},
	}

	for i, want := range wantOrder {
		got := pq.PopTask()
		if got.ID != want.id || got.Priority != want.priority {
			t.Fatalf("Pop #%d = %q(%d), want %q(%d)", i, got.ID, got.Priority, want.id, want.priority)
		}
	}

	if pq.Len() != 0 {
		t.Fatalf("Len() after draining = %d, want 0", pq.Len())
	}
}

func TestUpdatePriorityReordersQueue(t *testing.T) {
	pq := NewPriorityQueue()
	a := pq.PushTask("a", 10)
	pq.PushTask("b", 20)
	pq.PushTask("c", 30)

	// Promote 'a' to be the least urgent; 'b' should now come out first.
	pq.Update(a, 100)

	first := pq.PopTask()
	if first.ID != "b" {
		t.Fatalf("Pop() = %q, want %q after demoting 'a'", first.ID, "b")
	}
	second := pq.PopTask()
	if second.ID != "c" {
		t.Fatalf("Pop() = %q, want %q", second.ID, "c")
	}
	third := pq.PopTask()
	if third.ID != "a" || third.Priority != 100 {
		t.Fatalf("Pop() = %q(%d), want %q(100)", third.ID, third.Priority, "a")
	}
}

func TestPopOnEmptyPanics(t *testing.T) {
	pq := NewPriorityQueue()
	defer func() {
		if recover() == nil {
			t.Fatal("expected PopTask on empty queue to panic")
		}
	}()
	pq.PopTask()
}

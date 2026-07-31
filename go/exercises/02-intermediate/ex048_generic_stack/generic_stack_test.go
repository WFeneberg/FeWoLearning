package genericstack

import "testing"

func TestStackIntLIFO(t *testing.T) {
	s := NewStack[int]()

	if !s.IsEmpty() {
		t.Fatalf("new stack should be empty")
	}
	if _, ok := s.Pop(); ok {
		t.Fatalf("Pop on empty stack should return ok=false")
	}

	s.Push(1)
	s.Push(2)
	s.Push(3)

	if got := s.Len(); got != 3 {
		t.Fatalf("Len() = %d, want 3", got)
	}

	if peek, ok := s.Peek(); !ok || peek != 3 {
		t.Fatalf("Peek() = (%v, %v), want (3, true)", peek, ok)
	}

	wantOrder := []int{3, 2, 1}
	for _, want := range wantOrder {
		got, ok := s.Pop()
		if !ok {
			t.Fatalf("Pop() ok = false, want true")
		}
		if got != want {
			t.Fatalf("Pop() = %d, want %d", got, want)
		}
	}

	if !s.IsEmpty() {
		t.Fatalf("stack should be empty after popping all elements")
	}
	if got := s.Len(); got != 0 {
		t.Fatalf("Len() = %d, want 0", got)
	}
}

func TestStackStringLIFO(t *testing.T) {
	s := NewStack[string]()

	s.Push("a")
	s.Push("b")
	s.Push("c")

	if got := s.Len(); got != 3 {
		t.Fatalf("Len() = %d, want 3", got)
	}

	wantOrder := []string{"c", "b", "a"}
	for _, want := range wantOrder {
		got, ok := s.Pop()
		if !ok {
			t.Fatalf("Pop() ok = false, want true")
		}
		if got != want {
			t.Fatalf("Pop() = %q, want %q", got, want)
		}
	}

	if _, ok := s.Pop(); ok {
		t.Fatalf("Pop on empty stack should return ok=false")
	}
	if _, ok := s.Peek(); ok {
		t.Fatalf("Peek on empty stack should return ok=false")
	}
}

package stackops

import (
	"errors"
	"testing"
)

func TestStackLIFO(t *testing.T) {
	var s Stack
	s.Push(1)
	s.Push(2)
	s.Push(3)

	want := []int{3, 2, 1}
	for _, w := range want {
		got, err := s.Pop()
		if err != nil {
			t.Fatalf("Pop() unexpected error: %v", err)
		}
		if got != w {
			t.Errorf("Pop() = %d, want %d", got, w)
		}
	}
}

func TestStackPopEmpty(t *testing.T) {
	var s Stack
	_, err := s.Pop()
	if err == nil {
		t.Fatal("Pop() on empty stack: expected error, got nil")
	}
	if !errors.Is(err, ErrEmptyStack) {
		t.Errorf("Pop() error = %v, want ErrEmptyStack", err)
	}
}

func TestStackPushPopMixed(t *testing.T) {
	var s Stack
	s.Push(10)
	if got, err := s.Pop(); err != nil || got != 10 {
		t.Fatalf("Pop() = %d, %v, want 10, nil", got, err)
	}
	if _, err := s.Pop(); !errors.Is(err, ErrEmptyStack) {
		t.Fatalf("Pop() on drained stack: expected ErrEmptyStack, got %v", err)
	}
	s.Push(42)
	s.Push(99)
	if got, err := s.Pop(); err != nil || got != 99 {
		t.Fatalf("Pop() = %d, %v, want 99, nil", got, err)
	}
}

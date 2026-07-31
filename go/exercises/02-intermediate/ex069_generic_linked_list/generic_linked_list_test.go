package genericlinkedlist

import (
	"reflect"
	"testing"
)

func TestLinkedListInt(t *testing.T) {
	var l LinkedList[int]
	values := []int{1, 2, 3, 4, 5}
	for _, v := range values {
		l.Append(v)
	}
	if got := l.ToSlice(); !reflect.DeepEqual(got, values) {
		t.Errorf("ToSlice() = %v, want %v", got, values)
	}
	if got := l.Len(); got != len(values) {
		t.Errorf("Len() = %d, want %d", got, len(values))
	}
}

func TestLinkedListString(t *testing.T) {
	var l LinkedList[string]
	values := []string{"go", "is", "fun"}
	for _, v := range values {
		l.Append(v)
	}
	if got := l.ToSlice(); !reflect.DeepEqual(got, values) {
		t.Errorf("ToSlice() = %v, want %v", got, values)
	}
	if got := l.Len(); got != len(values) {
		t.Errorf("Len() = %d, want %d", got, len(values))
	}
}

func TestLinkedListEmpty(t *testing.T) {
	var l LinkedList[int]
	if got := l.ToSlice(); len(got) != 0 {
		t.Errorf("ToSlice() on empty list = %v, want empty", got)
	}
	if got := l.Len(); got != 0 {
		t.Errorf("Len() on empty list = %d, want 0", got)
	}
}

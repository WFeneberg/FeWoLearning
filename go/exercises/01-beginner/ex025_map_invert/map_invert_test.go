package mapinvert

import "testing"

func TestInvertUnique(t *testing.T) {
	in := map[string]int{"a": 1, "b": 2, "c": 3}
	got, err := Invert(in)
	if err != nil {
		t.Fatalf("Invert(%v) returned unexpected error: %v", in, err)
	}
	want := map[int]string{1: "a", 2: "b", 3: "c"}
	if len(got) != len(want) {
		t.Fatalf("Invert(%v) = %v, want %v", in, got, want)
	}
	for k, v := range want {
		if got[k] != v {
			t.Errorf("Invert(%v)[%d] = %q, want %q", in, k, got[k], v)
		}
	}
}

func TestInvertDuplicateValue(t *testing.T) {
	in := map[string]int{"a": 1, "b": 1, "c": 2}
	_, err := Invert(in)
	if err == nil {
		t.Fatalf("Invert(%v) = nil error, want non-nil error for duplicate values", in)
	}
}

func TestInvertEmpty(t *testing.T) {
	got, err := Invert(map[string]int{})
	if err != nil {
		t.Fatalf("Invert(empty) returned unexpected error: %v", err)
	}
	if len(got) != 0 {
		t.Errorf("Invert(empty) = %v, want empty map", got)
	}
}

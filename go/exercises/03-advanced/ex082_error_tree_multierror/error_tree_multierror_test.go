package errortreemultierror

import (
	"errors"
	"testing"
)

var (
	errSentinel = errors.New("connection refused")
	errUnrelated = errors.New("this sentinel is never aggregated")
)

func TestErrorsIsFindsSentinelBuriedInTree(t *testing.T) {
	// Build a tree:
	//   root
	//   ├── errors.New("disk full")
	//   ├── branch
	//   │   ├── errors.New("timeout")
	//   │   └── errSentinel        <- buried two levels deep
	//   └── errors.New("bad request")
	branch := Append(errors.New("timeout"), errSentinel)
	root := Append(errors.New("disk full"), branch, errors.New("bad request"))

	if !errors.Is(root, errSentinel) {
		t.Fatal("errors.Is(root, errSentinel) = false, want true (sentinel is buried in a nested MultiError)")
	}
	if errors.Is(root, errUnrelated) {
		t.Fatal("errors.Is(root, errUnrelated) = true, want false")
	}
}

func TestAppendFiltersNils(t *testing.T) {
	m := Append(nil, errors.New("a"), nil, errors.New("b"), nil)
	if m == nil {
		t.Fatal("Append() = nil, want non-nil MultiError")
	}
	if len(m.Errs) != 2 {
		t.Fatalf("len(m.Errs) = %d, want 2", len(m.Errs))
	}
	if got, want := m.Error(), "a; b"; got != want {
		t.Errorf("Error() = %q, want %q", got, want)
	}
}

func TestAppendAllNilReturnsNil(t *testing.T) {
	if m := Append(nil, nil); m != nil {
		t.Errorf("Append(nil, nil) = %v, want nil", m)
	}
	if m := Append(); m != nil {
		t.Errorf("Append() = %v, want nil", m)
	}
}

func TestUnwrapReturnsUnderlyingSlice(t *testing.T) {
	e1 := errors.New("one")
	e2 := errors.New("two")
	m := Append(e1, e2)

	got := m.Unwrap()
	if len(got) != 2 || got[0] != e1 || got[1] != e2 {
		t.Errorf("Unwrap() = %v, want [%v %v]", got, e1, e2)
	}
}

func TestErrorMessageJoinsNestedNodes(t *testing.T) {
	inner := Append(errors.New("x"), errors.New("y"))
	outer := Append(errors.New("start"), inner, errors.New("end"))

	want := "start; x; y; end"
	if got := outer.Error(); got != want {
		t.Errorf("Error() = %q, want %q", got, want)
	}
}

func TestAsFindsTypedErrorBuriedInTree(t *testing.T) {
	target := &customErr{msg: "missing config"}
	branch := Append(errors.New("noise"), target)
	root := Append(errors.New("more noise"), branch)

	var found *customErr
	if !errors.As(root, &found) {
		t.Fatal("errors.As(root, &found) = false, want true")
	}
	if found != target {
		t.Errorf("errors.As found %v, want %v", found, target)
	}
}

type customErr struct{ msg string }

func (e *customErr) Error() string { return e.msg }

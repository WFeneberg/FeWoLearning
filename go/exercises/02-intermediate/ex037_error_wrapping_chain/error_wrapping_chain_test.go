package errorwrappingchain

import (
	"errors"
	"strings"
	"testing"
)

func TestLayer3ReturnsSentinel(t *testing.T) {
	err := Layer3()
	if err == nil {
		t.Fatal("Layer3() = nil, want an error")
	}
	if !errors.Is(err, ErrSentinel) {
		t.Errorf("errors.Is(Layer3(), ErrSentinel) = false, want true")
	}
}

func TestLayer2WrapsSentinel(t *testing.T) {
	err := Layer2()
	if err == nil {
		t.Fatal("Layer2() = nil, want an error")
	}
	if !errors.Is(err, ErrSentinel) {
		t.Errorf("errors.Is(Layer2(), ErrSentinel) = false, want true")
	}
	if err == ErrSentinel {
		t.Errorf("Layer2() returned the bare sentinel; want it wrapped with context")
	}
}

func TestLayer1WrapsThroughAllLayers(t *testing.T) {
	err := Layer1()
	if err == nil {
		t.Fatal("Layer1() = nil, want an error")
	}
	if !errors.Is(err, ErrSentinel) {
		t.Errorf("errors.Is(Layer1(), ErrSentinel) = false, want true")
	}

	msg := err.Error()
	if !strings.Contains(msg, ErrSentinel.Error()) {
		t.Errorf("Layer1().Error() = %q, want it to contain %q", msg, ErrSentinel.Error())
	}

	// Unwrapping repeatedly must eventually reach ErrSentinel exactly.
	cur := err
	found := false
	for i := 0; i < 10 && cur != nil; i++ {
		if cur == ErrSentinel {
			found = true
			break
		}
		cur = errors.Unwrap(cur)
	}
	if !found {
		t.Errorf("unwrapping Layer1() never reached ErrSentinel")
	}
}

func TestErrorsIsFailsForUnrelatedError(t *testing.T) {
	other := errors.New("unrelated failure")
	if errors.Is(other, ErrSentinel) {
		t.Errorf("errors.Is(other, ErrSentinel) = true, want false")
	}
}

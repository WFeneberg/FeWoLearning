package customerroras

import (
	"errors"
	"testing"
)

func TestFetchResource_NotFound(t *testing.T) {
	cases := []struct {
		name string
		kind string
		id   string
	}{
		{"user not found", "user", "1"},
		{"order not found", "order", "999"},
	}

	for _, tc := range cases {
		t.Run(tc.name, func(t *testing.T) {
			err := FetchResource(tc.kind, tc.id)
			if err == nil {
				t.Fatalf("FetchResource(%q, %q) = nil, want error", tc.kind, tc.id)
			}

			var nfe *NotFoundError
			if !errors.As(err, &nfe) {
				t.Fatalf("errors.As(%v, &NotFoundError{}) = false, want true", err)
			}
			if nfe.Kind != tc.kind {
				t.Errorf("NotFoundError.Kind = %q, want %q", nfe.Kind, tc.kind)
			}
			if nfe.ID != tc.id {
				t.Errorf("NotFoundError.ID = %q, want %q", nfe.ID, tc.id)
			}
		})
	}
}

func TestFetchResource_Found(t *testing.T) {
	err := FetchResource("user", "42")
	if err != nil {
		t.Fatalf("FetchResource(%q, %q) = %v, want nil", "user", "42", err)
	}
}

func TestNotFoundError_ErrorMessage(t *testing.T) {
	err := FetchResource("widget", "7")
	var nfe *NotFoundError
	if !errors.As(err, &nfe) {
		t.Fatalf("errors.As failed to extract NotFoundError")
	}
	want := "widget with id \"7\" not found"
	if nfe.Error() != want {
		t.Errorf("NotFoundError.Error() = %q, want %q", nfe.Error(), want)
	}
}

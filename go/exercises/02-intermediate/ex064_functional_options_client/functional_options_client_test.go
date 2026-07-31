package functionaloptionsclient

import (
	"testing"
	"time"
)

func TestNewClientDefaults(t *testing.T) {
	c, err := NewClient()
	if err != nil {
		t.Fatalf("NewClient() unexpected error: %v", err)
	}
	if c.Timeout != 30*time.Second {
		t.Errorf("default Timeout = %v, want %v", c.Timeout, 30*time.Second)
	}
	if c.Retries != 3 {
		t.Errorf("default Retries = %d, want %d", c.Retries, 3)
	}
}

func TestNewClientAppliesOptions(t *testing.T) {
	c, err := NewClient(WithTimeout(5*time.Second), WithRetries(7))
	if err != nil {
		t.Fatalf("NewClient() unexpected error: %v", err)
	}
	if c.Timeout != 5*time.Second {
		t.Errorf("Timeout = %v, want %v", c.Timeout, 5*time.Second)
	}
	if c.Retries != 7 {
		t.Errorf("Retries = %d, want %d", c.Retries, 7)
	}
}

func TestNewClientInvalidRetries(t *testing.T) {
	c, err := NewClient(WithRetries(-1))
	if err == nil {
		t.Fatal("NewClient() with negative retries: expected error, got nil")
	}
	if c != nil {
		t.Errorf("NewClient() with error: expected nil client, got %+v", c)
	}
}

func TestNewClientInvalidTimeout(t *testing.T) {
	c, err := NewClient(WithTimeout(0))
	if err == nil {
		t.Fatal("NewClient() with zero timeout: expected error, got nil")
	}
	if c != nil {
		t.Errorf("NewClient() with error: expected nil client, got %+v", c)
	}
}

func TestNewClientStopsAtFirstError(t *testing.T) {
	// The valid WithRetries(9) before the invalid timeout should not leak
	// into the returned client, since construction fails as a whole.
	c, err := NewClient(WithRetries(9), WithTimeout(-1*time.Second))
	if err == nil {
		t.Fatal("NewClient() expected error for negative timeout")
	}
	if c != nil {
		t.Errorf("expected nil client on error, got %+v", c)
	}
}

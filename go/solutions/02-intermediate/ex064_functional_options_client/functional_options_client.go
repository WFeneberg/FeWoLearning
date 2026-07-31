// Package functionaloptionsclient — Exercise 064 (reference solution).
package functionaloptionsclient

import (
	"fmt"
	"time"
)

// Client is configured entirely through functional options.
type Client struct {
	Timeout time.Duration
	Retries int
}

// ClientOption configures a Client during construction. It returns an error
// if the option's value is invalid.
type ClientOption func(*Client) error

// WithTimeout sets the client's request timeout. The timeout must be
// strictly positive.
func WithTimeout(d time.Duration) ClientOption {
	return func(c *Client) error {
		if d <= 0 {
			return fmt.Errorf("functionaloptionsclient: timeout must be positive, got %v", d)
		}
		c.Timeout = d
		return nil
	}
}

// WithRetries sets the number of retry attempts. Retries must not be
// negative.
func WithRetries(n int) ClientOption {
	return func(c *Client) error {
		if n < 0 {
			return fmt.Errorf("functionaloptionsclient: retries must not be negative, got %d", n)
		}
		c.Retries = n
		return nil
	}
}

// NewClient builds a Client applying sensible defaults, then applies each
// option in order. If any option reports an error, NewClient returns that
// error and a nil *Client.
func NewClient(opts ...ClientOption) (*Client, error) {
	c := &Client{
		Timeout: 30 * time.Second,
		Retries: 3,
	}
	for _, opt := range opts {
		if err := opt(c); err != nil {
			return nil, err
		}
	}
	return c, nil
}

// Package functionaloptionsclient — Exercise 064 (intermediate).
// Goal:   Configure a Client via the functional options pattern, validating
//         each option and surfacing construction errors instead of panicking.
// Drills: functional options, variadic parameters, error wrapping.
package functionaloptionsclient

import "time"

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
	panic("TODO: implement WithTimeout")
}

// WithRetries sets the number of retry attempts. Retries must not be
// negative.
func WithRetries(n int) ClientOption {
	panic("TODO: implement WithRetries")
}

// NewClient builds a Client applying sensible defaults, then applies each
// option in order. If any option reports an error, NewClient returns that
// error and a nil *Client.
func NewClient(opts ...ClientOption) (*Client, error) {
	panic("TODO: implement NewClient")
}

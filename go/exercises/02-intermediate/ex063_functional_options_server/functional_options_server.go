// Package functionaloptionsserver — Exercise 063 (intermediate).
// Goal:   Implement NewServer(opts ...Option) *Server that configures a
//         Server's Host, Port, and Timeout via the functional options
//         pattern, applying sane defaults when no options are given.
// Drills: functional options, variadic parameters, closures over struct
//         fields, API design for extensible constructors.
package functionaloptionsserver

import "time"

// Server holds the configuration for a (mock) server.
type Server struct {
	Host    string
	Port    int
	Timeout time.Duration
}

// Option mutates a Server during construction.
type Option func(*Server)

// WithHost overrides the default host.
func WithHost(host string) Option {
	panic("TODO: implement WithHost")
}

// WithPort overrides the default port.
func WithPort(port int) Option {
	panic("TODO: implement WithPort")
}

// WithTimeout overrides the default timeout.
func WithTimeout(timeout time.Duration) Option {
	panic("TODO: implement WithTimeout")
}

// NewServer builds a Server with default values, applying any opts on top.
func NewServer(opts ...Option) *Server {
	panic("TODO: implement NewServer")
}

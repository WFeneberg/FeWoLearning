// Package functionaloptionsserver — Exercise 063 (reference solution).
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
	return func(s *Server) {
		s.Host = host
	}
}

// WithPort overrides the default port.
func WithPort(port int) Option {
	return func(s *Server) {
		s.Port = port
	}
}

// WithTimeout overrides the default timeout.
func WithTimeout(timeout time.Duration) Option {
	return func(s *Server) {
		s.Timeout = timeout
	}
}

// NewServer builds a Server with default values, applying any opts on top.
func NewServer(opts ...Option) *Server {
	s := &Server{
		Host:    "localhost",
		Port:    8080,
		Timeout: 30 * time.Second,
	}
	for _, opt := range opts {
		opt(s)
	}
	return s
}

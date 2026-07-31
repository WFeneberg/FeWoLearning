package functionaloptionsserver

import (
	"testing"
	"time"
)

func TestNewServerDefaults(t *testing.T) {
	s := NewServer()

	if s.Host != "localhost" {
		t.Errorf("Host = %q, want %q", s.Host, "localhost")
	}
	if s.Port != 8080 {
		t.Errorf("Port = %d, want %d", s.Port, 8080)
	}
	if s.Timeout != 30*time.Second {
		t.Errorf("Timeout = %v, want %v", s.Timeout, 30*time.Second)
	}
}

func TestNewServerWithOptions(t *testing.T) {
	cases := []struct {
		name string
		opts []Option
		want Server
	}{
		{
			name: "override host only",
			opts: []Option{WithHost("example.com")},
			want: Server{Host: "example.com", Port: 8080, Timeout: 30 * time.Second},
		},
		{
			name: "override port only",
			opts: []Option{WithPort(9090)},
			want: Server{Host: "localhost", Port: 9090, Timeout: 30 * time.Second},
		},
		{
			name: "override timeout only",
			opts: []Option{WithTimeout(5 * time.Second)},
			want: Server{Host: "localhost", Port: 8080, Timeout: 5 * time.Second},
		},
		{
			name: "override all",
			opts: []Option{WithHost("api.internal"), WithPort(443), WithTimeout(2 * time.Minute)},
			want: Server{Host: "api.internal", Port: 443, Timeout: 2 * time.Minute},
		},
		{
			name: "later option wins when repeated",
			opts: []Option{WithPort(1111), WithPort(2222)},
			want: Server{Host: "localhost", Port: 2222, Timeout: 30 * time.Second},
		},
	}

	for _, tc := range cases {
		t.Run(tc.name, func(t *testing.T) {
			got := NewServer(tc.opts...)
			if *got != tc.want {
				t.Errorf("NewServer(...) = %+v, want %+v", *got, tc.want)
			}
		})
	}
}

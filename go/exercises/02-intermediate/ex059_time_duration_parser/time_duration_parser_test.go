package timedurationparser

import (
	"testing"
	"time"
)

func TestFormatHumanDuration(t *testing.T) {
	cases := []struct {
		d    time.Duration
		want string
	}{
		{0, "0s"},
		{45 * time.Second, "45s"},
		{90 * time.Minute, "1h30m"},
		{time.Hour, "1h"},
		{2 * time.Hour, "2h"},
		{90 * time.Second, "1m30s"},
		{-90 * time.Minute, "-1h30m"},
		{3661 * time.Second, "1h1m1s"},
	}
	for _, c := range cases {
		if got := FormatHumanDuration(c.d); got != c.want {
			t.Errorf("FormatHumanDuration(%v) = %q, want %q", c.d, got, c.want)
		}
	}
}

func TestParseHumanDuration(t *testing.T) {
	cases := []struct {
		s    string
		want time.Duration
	}{
		{"0s", 0},
		{"45s", 45 * time.Second},
		{"1h30m", 90 * time.Minute},
		{"1h", time.Hour},
		{"1m30s", 90 * time.Second},
		{"-1h30m", -90 * time.Minute},
		{"1h1m1s", 3661 * time.Second},
	}
	for _, c := range cases {
		got, err := ParseHumanDuration(c.s)
		if err != nil {
			t.Fatalf("ParseHumanDuration(%q) returned error: %v", c.s, err)
		}
		if got != c.want {
			t.Errorf("ParseHumanDuration(%q) = %v, want %v", c.s, got, c.want)
		}
	}

	if _, err := ParseHumanDuration("garbage"); err == nil {
		t.Error("ParseHumanDuration(garbage) expected error, got nil")
	}
	if _, err := ParseHumanDuration(""); err == nil {
		t.Error("ParseHumanDuration(empty) expected error, got nil")
	}
}

func TestFormatParseRoundTrip(t *testing.T) {
	durations := []time.Duration{
		0,
		time.Second,
		45 * time.Second,
		90 * time.Minute,
		3661 * time.Second,
		-3661 * time.Second,
		25 * time.Hour,
	}
	for _, d := range durations {
		formatted := FormatHumanDuration(d)
		parsed, err := ParseHumanDuration(formatted)
		if err != nil {
			t.Fatalf("ParseHumanDuration(%q) returned error: %v", formatted, err)
		}
		if parsed != d {
			t.Errorf("round trip for %v: formatted=%q parsed=%v, want %v", d, formatted, parsed, d)
		}
	}
}

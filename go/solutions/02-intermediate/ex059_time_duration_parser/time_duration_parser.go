// Package timedurationparser — Exercise 059 (reference solution).
package timedurationparser

import (
	"fmt"
	"strconv"
	"strings"
	"time"
)

// FormatHumanDuration renders d as a compact string using only the units
// h (hours), m (minutes) and s (seconds), omitting any unit whose value is
// zero. Negative durations are prefixed with "-". A zero duration renders
// as "0s".
func FormatHumanDuration(d time.Duration) string {
	if d == 0 {
		return "0s"
	}

	neg := d < 0
	if neg {
		d = -d
	}

	hours := d / time.Hour
	d -= hours * time.Hour
	minutes := d / time.Minute
	d -= minutes * time.Minute
	seconds := d / time.Second

	var b strings.Builder
	if neg {
		b.WriteByte('-')
	}
	if hours > 0 {
		fmt.Fprintf(&b, "%dh", hours)
	}
	if minutes > 0 {
		fmt.Fprintf(&b, "%dm", minutes)
	}
	if seconds > 0 {
		fmt.Fprintf(&b, "%ds", seconds)
	}
	return b.String()
}

// ParseHumanDuration parses a string produced by FormatHumanDuration (or any
// combination of non-negative integer h/m/s components in that order, e.g.
// "1h30m", "90m", "45s") back into a time.Duration. It returns an error for
// malformed input.
func ParseHumanDuration(s string) (time.Duration, error) {
	orig := s
	if s == "" {
		return 0, fmt.Errorf("parse duration %q: empty string", orig)
	}

	neg := false
	if s[0] == '-' {
		neg = true
		s = s[1:]
	}
	if s == "" {
		return 0, fmt.Errorf("parse duration %q: missing value after sign", orig)
	}

	units := []struct {
		suffix string
		unit   time.Duration
	}{
		{"h", time.Hour},
		{"m", time.Minute},
		{"s", time.Second},
	}

	var total time.Duration
	found := false
	for _, u := range units {
		idx := strings.Index(s, u.suffix)
		if idx < 0 {
			continue
		}
		numStr := s[:idx]
		if numStr == "" {
			return 0, fmt.Errorf("parse duration %q: missing number before %q", orig, u.suffix)
		}
		n, err := strconv.ParseInt(numStr, 10, 64)
		if err != nil {
			return 0, fmt.Errorf("parse duration %q: %w", orig, err)
		}
		total += time.Duration(n) * u.unit
		found = true
		s = s[idx+1:]
	}

	if !found || s != "" {
		return 0, fmt.Errorf("parse duration %q: invalid format", orig)
	}

	if neg {
		total = -total
	}
	return total, nil
}

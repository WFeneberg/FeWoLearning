// Package timedurationparser — Exercise 059 (intermediate).
// Goal:   Format a time.Duration into a compact human string like "1h30m" or
//         "45s", and parse that format back into an equal time.Duration.
// Drills: time.Duration arithmetic, string building, custom parsing, strconv.
package timedurationparser

import "time"

// FormatHumanDuration renders d as a compact string using only the units
// h (hours), m (minutes) and s (seconds), omitting any unit whose value is
// zero. Negative durations are prefixed with "-". A zero duration renders
// as "0s".
func FormatHumanDuration(d time.Duration) string {
	panic("TODO: implement FormatHumanDuration")
}

// ParseHumanDuration parses a string produced by FormatHumanDuration (or any
// combination of non-negative integer h/m/s components in that order, e.g.
// "1h30m", "90m", "45s") back into a time.Duration. It returns an error for
// malformed input.
func ParseHumanDuration(s string) (time.Duration, error) {
	panic("TODO: implement ParseHumanDuration")
}

// Package timebusinessdays — Exercise 060 (reference solution).
package timebusinessdays

import "time"

// BusinessDaysBetween returns the number of weekdays (Mon-Fri) in the
// half-open interval [start, end) — i.e. it counts start's day but not
// end's day, comparing calendar dates only (time-of-day is ignored).
// If end is not after start, it returns 0.
func BusinessDaysBetween(start, end time.Time) int {
	start = time.Date(start.Year(), start.Month(), start.Day(), 0, 0, 0, 0, time.UTC)
	end = time.Date(end.Year(), end.Month(), end.Day(), 0, 0, 0, 0, time.UTC)

	if !end.After(start) {
		return 0
	}

	count := 0
	for d := start; d.Before(end); d = d.AddDate(0, 0, 1) {
		switch d.Weekday() {
		case time.Saturday, time.Sunday:
			// skip weekends
		default:
			count++
		}
	}
	return count
}

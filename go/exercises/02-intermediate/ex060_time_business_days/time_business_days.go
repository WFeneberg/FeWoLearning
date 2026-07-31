// Package timebusinessdays — Exercise 060 (intermediate).
// Goal:   Count business days (Mon-Fri) strictly between two dates,
//         excluding weekends.
// Drills: time.Time arithmetic, weekday handling, date normalization.
package timebusinessdays

import "time"

// BusinessDaysBetween returns the number of weekdays (Mon-Fri) in the
// half-open interval [start, end) — i.e. it counts start's day but not
// end's day, comparing calendar dates only (time-of-day is ignored).
// If end is not after start, it returns 0.
func BusinessDaysBetween(start, end time.Time) int {
	panic("TODO: implement BusinessDaysBetween")
}

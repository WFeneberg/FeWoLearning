package timebusinessdays

import (
	"testing"
	"time"
)

func date(y int, m time.Month, d int) time.Time {
	return time.Date(y, m, d, 0, 0, 0, 0, time.UTC)
}

func TestBusinessDaysBetween(t *testing.T) {
	cases := []struct {
		name       string
		start, end time.Time
		want       int
	}{
		{
			// Mon 2026-07-06 through Fri 2026-07-10 (inclusive start, exclusive end
			// at the following Monday) spans exactly one full business week.
			name:  "single full week",
			start: date(2026, time.July, 6),
			end:   date(2026, time.July, 13),
			want:  5,
		},
		{
			// Sat -> next Sat: no weekdays counted at all.
			name:  "weekend to weekend",
			start: date(2026, time.July, 11),
			end:   date(2026, time.July, 18),
			want:  5,
		},
		{
			// Spans three weekends: 2026-07-01 (Wed) through 2026-07-20 (Mon).
			name:  "multiple weekends",
			start: date(2026, time.July, 1),
			end:   date(2026, time.July, 20),
			want:  13,
		},
		{
			name:  "same day",
			start: date(2026, time.July, 15),
			end:   date(2026, time.July, 15),
			want:  0,
		},
		{
			name:  "end before start returns zero",
			start: date(2026, time.July, 15),
			end:   date(2026, time.July, 10),
			want:  0,
		},
		{
			// Time-of-day should be ignored: start at 23:00 counts its own day.
			name:  "time of day ignored",
			start: time.Date(2026, time.July, 6, 23, 0, 0, 0, time.UTC),
			end:   time.Date(2026, time.July, 7, 1, 0, 0, 0, time.UTC),
			want:  1,
		},
	}

	for _, c := range cases {
		t.Run(c.name, func(t *testing.T) {
			if got := BusinessDaysBetween(c.start, c.end); got != c.want {
				t.Errorf("BusinessDaysBetween(%v, %v) = %d, want %d", c.start, c.end, got, c.want)
			}
		})
	}
}

// Package selecttimeout — Exercise 044 (reference solution).
package selecttimeout

import "time"

func ReceiveWithTimeout(ch <-chan int, d time.Duration) (int, bool) {
	select {
	case v := <-ch:
		return v, true
	case <-time.After(d):
		return 0, false
	}
}

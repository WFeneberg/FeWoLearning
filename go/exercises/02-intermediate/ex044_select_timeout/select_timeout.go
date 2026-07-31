// Package selecttimeout — Exercise 044 (intermediate).
// Goal:   Receive a value from a channel, but give up if nothing arrives
//         within a given duration.
// Drills: select, time.After, channel receive with ok idiom.
package selecttimeout

import "time"

// ReceiveWithTimeout waits for a value on ch. If a value arrives before d
// elapses, it returns (value, true). If d elapses first, it returns (0, false).
func ReceiveWithTimeout(ch <-chan int, d time.Duration) (int, bool) {
	panic("TODO: implement ReceiveWithTimeout")
}

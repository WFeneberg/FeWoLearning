// Package selectmultiplex — Exercise 045 (intermediate).
// Goal:   Consume values from two channels concurrently until a total number
//         of values has been received, tracking how many came from each.
// Drills: select, channel multiplexing, non-deterministic fan-in.
package selectmultiplex

// CountFromBoth reads from a and b using select until total values have been
// received across both channels combined. It returns a map with keys "a" and
// "b" holding the count of values received from each channel.
func CountFromBoth(a, b <-chan int, total int) map[string]int {
	panic("TODO: implement CountFromBoth")
}

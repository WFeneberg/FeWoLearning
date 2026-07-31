// Package selectmultiplex — Exercise 045 (reference solution).
package selectmultiplex

// CountFromBoth reads from a and b using select until total values have been
// received across both channels combined. It returns a map with keys "a" and
// "b" holding the count of values received from each channel.
func CountFromBoth(a, b <-chan int, total int) map[string]int {
	counts := map[string]int{"a": 0, "b": 0}
	received := 0

	for received < total {
		select {
		case v, ok := <-a:
			if !ok {
				a = nil
				continue
			}
			_ = v
			counts["a"]++
			received++
		case v, ok := <-b:
			if !ok {
				b = nil
				continue
			}
			_ = v
			counts["b"]++
			received++
		}
	}

	return counts
}

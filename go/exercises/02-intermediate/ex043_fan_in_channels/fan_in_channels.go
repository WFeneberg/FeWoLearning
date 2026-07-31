// Package faninchannels — Exercise 043 (intermediate).
// Goal:   Fan multiple read-only int channels into a single merged output
//         channel that closes once all inputs are drained.
// Drills: goroutines, channels, sync.WaitGroup, fan-in pattern.
package faninchannels

// Merge fans in values from all chans into a single returned channel.
// The returned channel is closed once every input channel has been
// drained and closed.
func Merge(chans ...<-chan int) <-chan int {
	panic("TODO: implement Merge")
}

// Package channelpipeline — Exercise 042 (intermediate).
// Goal:   Build a two-stage pipeline: a generator stage that streams a
//         sequence of ints over a channel, and a squaring stage that reads
//         from the generator channel and emits the square of each value.
// Drills: channels, pipelines, goroutines, channel direction types.
package channelpipeline

// Generate starts a goroutine that sends each value of nums (in order) on
// the returned channel, then closes it.
func Generate(nums []int) <-chan int {
	panic("TODO: implement Generate")
}

// Square starts a goroutine that reads values from in, sends their square on
// the returned channel, and closes the returned channel once in is closed
// and drained.
func Square(in <-chan int) <-chan int {
	panic("TODO: implement Square")
}

// Pipeline wires Generate and Square together and collects all results from
// the final stage into a slice, preserving order.
func Pipeline(nums []int) []int {
	panic("TODO: implement Pipeline")
}

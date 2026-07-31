// Package channelpipeline — Exercise 042 (reference solution).
package channelpipeline

// Generate starts a goroutine that sends each value of nums (in order) on
// the returned channel, then closes it.
func Generate(nums []int) <-chan int {
	out := make(chan int)
	go func() {
		defer close(out)
		for _, n := range nums {
			out <- n
		}
	}()
	return out
}

// Square starts a goroutine that reads values from in, sends their square on
// the returned channel, and closes the returned channel once in is closed
// and drained.
func Square(in <-chan int) <-chan int {
	out := make(chan int)
	go func() {
		defer close(out)
		for n := range in {
			out <- n * n
		}
	}()
	return out
}

// Pipeline wires Generate and Square together and collects all results from
// the final stage into a slice, preserving order.
func Pipeline(nums []int) []int {
	result := make([]int, 0, len(nums))
	for v := range Square(Generate(nums)) {
		result = append(result, v)
	}
	return result
}

// Package slicechunk — Exercise 031 (reference solution).
package slicechunk

// Chunk splits nums into consecutive chunks of the given size.
// The last chunk may be shorter than size if len(nums) is not evenly
// divisible by size.
func Chunk(nums []int, size int) [][]int {
	chunks := [][]int{}
	for i := 0; i < len(nums); i += size {
		end := i + size
		if end > len(nums) {
			end = len(nums)
		}
		chunks = append(chunks, nums[i:end])
	}
	return chunks
}

// Package mapinvert — Exercise 025 (beginner).
// Goal:   Invert a map[string]int into a map[int]string, returning an error
//         when two keys share the same value (which would collide).
// Drills: maps, error handling, fmt.Errorf.
package mapinvert

// Invert swaps the keys and values of m. It returns an error if two keys map
// to the same value, since that would produce a collision in the result.
func Invert(m map[string]int) (map[int]string, error) {
	panic("TODO: implement Invert")
}

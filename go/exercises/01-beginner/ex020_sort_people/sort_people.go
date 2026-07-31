// Package sortpeople — Exercise 020 (beginner).
// Goal:   Sort a slice of Person structs by age, ascending, using sort.Slice.
// Drills: sort.Slice, structs, closures.
package sortpeople

// Person represents a named individual with an age.
type Person struct {
	Name string
	Age  int
}

// SortByAge sorts people by Age ascending, in place.
func SortByAge(people []Person) {
	panic("TODO: implement SortByAge")
}

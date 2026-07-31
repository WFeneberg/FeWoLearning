// Package sortpeople — Exercise 020 (reference solution).
package sortpeople

import "sort"

// Person represents a named individual with an age.
type Person struct {
	Name string
	Age  int
}

// SortByAge sorts people by Age ascending, in place.
func SortByAge(people []Person) {
	sort.Slice(people, func(i, j int) bool {
		return people[i].Age < people[j].Age
	})
}

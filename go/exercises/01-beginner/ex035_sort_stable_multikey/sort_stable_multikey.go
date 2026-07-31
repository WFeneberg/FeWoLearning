// Package sortstablemultikey — Exercise 035 (beginner).
// Goal:   Sort students by grade ascending, using sort.Stable so that
//         students with the same grade keep their original relative order.
// Drills: sort.Stable, sort.Interface, multi-key ordering.
package sortstablemultikey

// Student represents a single student record.
type Student struct {
	Name  string
	Grade int
}

// SortByGradeThenName sorts students by Grade ascending using sort.Stable,
// so that students sharing the same grade retain their original relative order.
func SortByGradeThenName(students []Student) {
	panic("TODO: implement SortByGradeThenName")
}

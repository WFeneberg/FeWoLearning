// Package fmttable — Exercise 017 (beginner).
// Goal:   Format a slice of Person into an aligned column table using
//         fmt.Sprintf with fixed-width verbs.
// Drills: fmt formatting, Sprintf width specifiers, strings.Builder.
package fmttable

// Person holds a name and an age for table formatting.
type Person struct {
	Name string
	Age  int
}

// FormatTable returns rows formatted as an aligned "Name  Age" table.
func FormatTable(rows []Person) string {
	panic("TODO: implement FormatTable")
}

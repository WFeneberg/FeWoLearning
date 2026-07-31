// Package fmttable — Exercise 017 (reference solution).
package fmttable

import (
	"fmt"
	"strings"
)

// Person holds a name and an age for table formatting.
type Person struct {
	Name string
	Age  int
}

// FormatTable returns rows formatted as an aligned "Name  Age" table.
func FormatTable(rows []Person) string {
	var sb strings.Builder
	sb.WriteString(fmt.Sprintf("%-10s%3s\n", "Name", "Age"))
	for _, p := range rows {
		sb.WriteString(fmt.Sprintf("%-10s%3d\n", p.Name, p.Age))
	}
	return sb.String()
}

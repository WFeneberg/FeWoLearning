// Package regexpemailvalidator — Exercise 061 (reference solution).
package regexpemailvalidator

import "regexp"

// emailPattern matches a reasonably strict email-like string:
// local part, "@", domain labels separated by single dots, and a
// final label of at least two letters. Consecutive dots and spaces
// are rejected.
var emailPattern = regexp.MustCompile(
	`^[A-Za-z0-9._%+-]+@[A-Za-z0-9]+(?:[.-][A-Za-z0-9]+)*\.[A-Za-z]{2,}$`,
)

// IsValidEmail reports whether s looks like a well-formed email address.
func IsValidEmail(s string) bool {
	return emailPattern.MatchString(s)
}

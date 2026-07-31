// Package customerrortype — Exercise 029 (reference solution).
package customerrortype

import "fmt"

// ValidationError reports that a named field failed validation.
type ValidationError struct {
	Field string
}

// Error implements the error interface.
func (e *ValidationError) Error() string {
	return fmt.Sprintf("validation failed for field %q", e.Field)
}

// ValidateAge returns a *ValidationError (as error) if age is negative,
// otherwise nil.
func ValidateAge(age int) error {
	if age < 0 {
		return &ValidationError{Field: "age"}
	}
	return nil
}

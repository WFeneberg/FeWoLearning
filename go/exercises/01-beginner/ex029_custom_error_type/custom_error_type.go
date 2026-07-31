// Package customerrortype — Exercise 029 (beginner).
// Goal:   Define a ValidationError type implementing the error interface
//         with a Field name, and recover it from a failing validation via
//         a type assertion.
// Drills: custom error types, error interface, type assertion.
package customerrortype

// ValidationError reports that a named field failed validation.
type ValidationError struct {
	Field string
}

// Error implements the error interface.
func (e *ValidationError) Error() string {
	panic("TODO: implement Error")
}

// ValidateAge returns a *ValidationError (as error) if age is negative,
// otherwise nil.
func ValidateAge(age int) error {
	panic("TODO: implement ValidateAge")
}

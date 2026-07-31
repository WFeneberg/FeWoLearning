// Package tabledrivencalculator — Exercise 058 (reference solution).
package tabledrivencalculator

import "fmt"

// Calculate applies the named operator ("add", "sub", "mul", "div") to a and
// b. It returns an error for an unknown operator or division by zero.
func Calculate(op string, a, b float64) (float64, error) {
	switch op {
	case "add":
		return a + b, nil
	case "sub":
		return a - b, nil
	case "mul":
		return a * b, nil
	case "div":
		if b == 0 {
			return 0, fmt.Errorf("division by zero")
		}
		return a / b, nil
	default:
		return 0, fmt.Errorf("unknown operator: %q", op)
	}
}

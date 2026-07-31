// Package dividesafe — Exercise 012 (reference solution).
package dividesafe

import "errors"

// Divide returns a divided by b, or an error if b is zero.
func Divide(a, b int) (int, error) {
	if b == 0 {
		return 0, errors.New("dividesafe: division by zero")
	}
	return a / b, nil
}

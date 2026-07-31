// Package jsonmarshalstruct — Exercise 051 (reference solution).
package jsonmarshalstruct

import "encoding/json"

// Product represents an item with a name, price, and stock quantity.
type Product struct {
	Name     string  `json:"name"`
	Price    float64 `json:"price"`
	InStock  bool    `json:"in_stock"`
	Quantity int     `json:"quantity"`
}

// ToJSON returns the JSON encoding of p as a string.
func ToJSON(p Product) (string, error) {
	b, err := json.Marshal(p)
	if err != nil {
		return "", err
	}
	return string(b), nil
}

// Package jsonmarshalstruct — Exercise 051 (intermediate).
// Goal:   Define a Product struct with json tags and implement ToJSON
//         returning the exact JSON string produced by json.Marshal.
// Drills: encoding/json, struct tags, marshaling.
package jsonmarshalstruct

// Product represents an item with a name, price, and stock quantity.
type Product struct {
	Name     string  `json:"name"`
	Price    float64 `json:"price"`
	InStock  bool    `json:"in_stock"`
	Quantity int     `json:"quantity"`
}

// ToJSON returns the JSON encoding of p as a string.
func ToJSON(p Product) (string, error) {
	panic("TODO: implement ToJSON")
}

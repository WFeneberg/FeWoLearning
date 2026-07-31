// Package jsonunmarshalnested — Exercise 052 (intermediate).
// Goal:   Unmarshal nested JSON (an order containing a slice of line items)
//         into structs and return the populated value.
// Drills: encoding/json, nested structs, struct tags, error handling.
package jsonunmarshalnested

// LineItem represents a single item within an order.
type LineItem struct {
	SKU      string  `json:"sku"`
	Name     string  `json:"name"`
	Quantity int     `json:"quantity"`
	Price    float64 `json:"price"`
}

// Customer holds the customer details attached to an order.
type Customer struct {
	Name  string `json:"name"`
	Email string `json:"email"`
}

// Order represents a customer order with nested line items.
type Order struct {
	ID       string     `json:"id"`
	Customer Customer   `json:"customer"`
	Items    []LineItem `json:"items"`
	Total    float64    `json:"total"`
}

// ParseOrder unmarshals data into an Order, returning an error if the JSON
// is malformed.
func ParseOrder(data []byte) (Order, error) {
	panic("TODO: implement ParseOrder")
}

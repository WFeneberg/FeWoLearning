package jsonunmarshalnested

import "testing"

func TestParseOrder(t *testing.T) {
	data := []byte(`{
		"id": "ORD-1001",
		"customer": {
			"name": "Ada Lovelace",
			"email": "ada@example.com"
		},
		"items": [
			{"sku": "SKU-1", "name": "Widget", "quantity": 2, "price": 9.99},
			{"sku": "SKU-2", "name": "Gadget", "quantity": 1, "price": 19.5}
		],
		"total": 39.48
	}`)

	order, err := ParseOrder(data)
	if err != nil {
		t.Fatalf("ParseOrder returned unexpected error: %v", err)
	}

	if order.ID != "ORD-1001" {
		t.Errorf("ID = %q, want %q", order.ID, "ORD-1001")
	}
	if order.Customer.Name != "Ada Lovelace" {
		t.Errorf("Customer.Name = %q, want %q", order.Customer.Name, "Ada Lovelace")
	}
	if order.Customer.Email != "ada@example.com" {
		t.Errorf("Customer.Email = %q, want %q", order.Customer.Email, "ada@example.com")
	}
	if order.Total != 39.48 {
		t.Errorf("Total = %v, want %v", order.Total, 39.48)
	}

	if len(order.Items) != 2 {
		t.Fatalf("len(Items) = %d, want 2", len(order.Items))
	}

	first := order.Items[0]
	if first.SKU != "SKU-1" || first.Name != "Widget" || first.Quantity != 2 || first.Price != 9.99 {
		t.Errorf("Items[0] = %+v, want {SKU-1 Widget 2 9.99}", first)
	}

	second := order.Items[1]
	if second.SKU != "SKU-2" || second.Name != "Gadget" || second.Quantity != 1 || second.Price != 19.5 {
		t.Errorf("Items[1] = %+v, want {SKU-2 Gadget 1 19.5}", second)
	}
}

func TestParseOrderInvalidJSON(t *testing.T) {
	_, err := ParseOrder([]byte(`{"id": "ORD-1001",`))
	if err == nil {
		t.Fatal("expected an error for malformed JSON, got nil")
	}
}

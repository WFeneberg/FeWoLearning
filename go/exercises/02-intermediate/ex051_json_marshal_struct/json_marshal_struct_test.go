package jsonmarshalstruct

import "testing"

func TestToJSON(t *testing.T) {
	cases := []struct {
		name string
		p    Product
		want string
	}{
		{
			name: "in stock item",
			p:    Product{Name: "Widget", Price: 9.99, InStock: true, Quantity: 42},
			want: `{"name":"Widget","price":9.99,"in_stock":true,"quantity":42}`,
		},
		{
			name: "out of stock item with zero price",
			p:    Product{Name: "Gadget", Price: 0, InStock: false, Quantity: 0},
			want: `{"name":"Gadget","price":0,"in_stock":false,"quantity":0}`,
		},
		{
			name: "empty name",
			p:    Product{Name: "", Price: 100.5, InStock: true, Quantity: 1},
			want: `{"name":"","price":100.5,"in_stock":true,"quantity":1}`,
		},
	}

	for _, tc := range cases {
		t.Run(tc.name, func(t *testing.T) {
			got, err := ToJSON(tc.p)
			if err != nil {
				t.Fatalf("ToJSON(%+v) returned error: %v", tc.p, err)
			}
			if got != tc.want {
				t.Errorf("ToJSON(%+v) = %q, want %q", tc.p, got, tc.want)
			}
		})
	}
}

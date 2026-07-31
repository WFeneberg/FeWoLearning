package jsonstreamdecoder

import (
	"errors"
	"strings"
	"testing"
)

func TestDecodeStream(t *testing.T) {
	cases := []struct {
		name    string
		input   string
		want    []Item
		wantErr bool
	}{
		{
			name:  "multiple items in order",
			input: `[{"id":1,"name":"alpha"},{"id":2,"name":"beta"},{"id":3,"name":"gamma"}]`,
			want: []Item{
				{ID: 1, Name: "alpha"},
				{ID: 2, Name: "beta"},
				{ID: 3, Name: "gamma"},
			},
		},
		{
			name:  "empty array",
			input: `[]`,
			want:  []Item{},
		},
		{
			name:  "single item",
			input: `[{"id":42,"name":"solo"}]`,
			want:  []Item{{ID: 42, Name: "solo"}},
		},
		{
			name:  "whitespace and newlines between items",
			input: "[\n  {\"id\":1,\"name\":\"a\"},\n  {\"id\":2,\"name\":\"b\"}\n]\n",
			want: []Item{
				{ID: 1, Name: "a"},
				{ID: 2, Name: "b"},
			},
		},
		{
			name:    "not an array",
			input:   `{"id":1,"name":"alpha"}`,
			wantErr: true,
		},
		{
			name:    "malformed json",
			input:   `[{"id":1,"name":"alpha"}`,
			wantErr: true,
		},
	}

	for _, tc := range cases {
		t.Run(tc.name, func(t *testing.T) {
			got, err := DecodeStream(strings.NewReader(tc.input))
			if tc.wantErr {
				if err == nil {
					t.Fatalf("DecodeStream(%q) expected error, got nil", tc.input)
				}
				return
			}
			if err != nil {
				t.Fatalf("DecodeStream(%q) unexpected error: %v", tc.input, err)
			}
			if len(got) != len(tc.want) {
				t.Fatalf("DecodeStream(%q) = %v, want %v", tc.input, got, tc.want)
			}
			for i := range got {
				if got[i] != tc.want[i] {
					t.Errorf("DecodeStream(%q)[%d] = %+v, want %+v", tc.input, i, got[i], tc.want[i])
				}
			}
		})
	}
}

func TestDecodeStreamErrorIsNotNilSentinel(t *testing.T) {
	_, err := DecodeStream(strings.NewReader(`not json at all`))
	if err == nil {
		t.Fatal("expected error for invalid json input")
	}
	if errors.Is(err, nil) {
		t.Fatal("unexpected sentinel comparison behavior")
	}
}

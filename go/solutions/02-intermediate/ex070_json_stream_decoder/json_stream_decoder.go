// Package jsonstreamdecoder — Exercise 070 (reference solution).
package jsonstreamdecoder

import (
	"encoding/json"
	"fmt"
	"io"
)

// Item is a single element of the streamed JSON array.
type Item struct {
	ID   int    `json:"id"`
	Name string `json:"name"`
}

// DecodeStream reads a JSON array of Item from r using a streaming
// json.Decoder (Token + Decode), returning the items in order.
func DecodeStream(r io.Reader) ([]Item, error) {
	dec := json.NewDecoder(r)

	// Consume the opening '[' token of the array.
	tok, err := dec.Token()
	if err != nil {
		return nil, fmt.Errorf("reading opening token: %w", err)
	}
	delim, ok := tok.(json.Delim)
	if !ok || delim != '[' {
		return nil, fmt.Errorf("expected array, got %v", tok)
	}

	items := []Item{}
	for dec.More() {
		var item Item
		if err := dec.Decode(&item); err != nil {
			return nil, fmt.Errorf("decoding item: %w", err)
		}
		items = append(items, item)
	}

	// Consume the closing ']' token.
	tok, err = dec.Token()
	if err != nil {
		return nil, fmt.Errorf("reading closing token: %w", err)
	}
	delim, ok = tok.(json.Delim)
	if !ok || delim != ']' {
		return nil, fmt.Errorf("expected end of array, got %v", tok)
	}

	return items, nil
}

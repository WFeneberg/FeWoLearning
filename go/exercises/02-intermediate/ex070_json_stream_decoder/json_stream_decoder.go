// Package jsonstreamdecoder — Exercise 070 (intermediate).
// Goal:   Implement DecodeStream(r io.Reader) ([]Item, error) using
//         json.Decoder.Token/Decode to incrementally read a large JSON
//         array without loading it all via json.Unmarshal.
// Drills: encoding/json streaming API, json.Decoder, io.Reader, json.Token.
package jsonstreamdecoder

import "io"

// Item is a single element of the streamed JSON array.
type Item struct {
	ID   int    `json:"id"`
	Name string `json:"name"`
}

// DecodeStream reads a JSON array of Item from r using a streaming
// json.Decoder (Token + Decode), returning the items in order.
// It must not read the entire input into memory via json.Unmarshal.
func DecodeStream(r io.Reader) ([]Item, error) {
	panic("TODO: implement DecodeStream")
}

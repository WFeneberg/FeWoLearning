// Exercise 089 — parsing a binary record (advanced).
// Goal:   read a real wire format, byte by byte.
// Drills: DataView with a moving offset, a length-prefixed string, a
//         repeated field, TextEncoder/TextDecoder over a byte range, and
//         a truncation check that fails loudly.
// Passes: parse(encode(x)) === x for every record, and a buffer cut short
//         throws a RangeError rather than returning nonsense.
//
// The format, all big-endian:
//   uint16  version
//   uint8   nameLength
//   bytes   name, UTF-8, nameLength bytes
//   uint16  tagCount
//   for each tag: uint8 length, then that many UTF-8 bytes

/**
 * Encodes { version, name, tags } into an ArrayBuffer in the format above.
 *
 * TODO: implement.
 */
export function encodeRecord(_record) {
  throw new Error("TODO: implement encodeRecord");
}

/**
 * Decodes such a buffer back into { version, name, tags }.
 *
 * A buffer too short for what its own headers claim must throw a
 * RangeError — reading past the end of a DataView already does, so the
 * job is not to swallow it.
 *
 * TODO: implement.
 */
export function decodeRecord(_buffer) {
  throw new Error("TODO: implement decodeRecord");
}

/**
 * The byte length `record` will encode to, computed WITHOUT encoding it
 * twice — the header sizes plus the UTF-8 byte length of every string.
 *
 * TODO: implement.
 */
export function encodedLength(_record) {
  throw new Error("TODO: implement encodedLength");
}

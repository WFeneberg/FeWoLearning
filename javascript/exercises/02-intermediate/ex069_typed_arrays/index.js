// Exercise 069 — typed arrays and DataView (intermediate).
// Goal:   lay bytes out on purpose.
// Drills: ArrayBuffer vs its views, Uint8Array, DataView's explicit
//         endianness, several views over ONE buffer.
// Passes: packRecord/unpackRecord round-trip big-endian, and
//         sharedBuffer() shows a write through one view appearing in
//         another.

/**
 * Packs { id (uint32), score (float64) } into a 12-byte ArrayBuffer,
 * BIG-endian, id first.
 *
 * TODO: implement with DataView.
 */
export function packRecord(_id, _score) {
  throw new Error("TODO: implement packRecord");
}

/**
 * The inverse: reads { id, score } back out of such a buffer.
 *
 * TODO: implement.
 */
export function unpackRecord(_buffer) {
  throw new Error("TODO: implement unpackRecord");
}

/**
 * Writes 0x01020304 as a uint32 twice into an 8-byte buffer — once
 * big-endian at offset 0, once little-endian at offset 4 — and returns all
 * eight bytes as a plain array.
 *
 * TODO: implement. The two halves are mirror images, which is what
 * endianness IS.
 */
export function endiannessBytes() {
  throw new Error("TODO: implement endiannessBytes");
}

/**
 * Creates one 4-byte buffer with a Uint8Array view and a Uint32Array view
 * over it, writes 255 into byte 0 through the byte view, and returns
 *   { bytes: [...], asUint32 }
 * read back afterwards — one piece of memory, two interpretations.
 *
 * TODO: implement. (Uint32Array uses the platform's own endianness, which
 * is little-endian here — that is exactly why DataView exists.)
 */
export function sharedBuffer() {
  throw new Error("TODO: implement sharedBuffer");
}

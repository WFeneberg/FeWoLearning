// Exercise 068 — text and bytes (intermediate).
// Goal:   stop conflating characters, code units and bytes.
// Drills: TextEncoder/TextDecoder, UTF-8 byte length, String#length as
//         UTF-16 CODE UNITS, iterating by code point.
// Passes: measure("😀") reports 2 code units, 1 code point and 4 bytes —
//         three different numbers for one character.

/** UTF-8 bytes of a string, as a Uint8Array. TODO: implement. */
export function encodeUtf8(_text) {
  throw new Error("TODO: implement encodeUtf8");
}

/** The string those bytes encode. TODO: implement. */
export function decodeUtf8(_bytes) {
  throw new Error("TODO: implement decodeUtf8");
}

/**
 * { codeUnits, codePoints, bytes } for a string:
 *   codeUnits  — what .length reports
 *   codePoints — what iterating the string yields
 *   bytes      — its UTF-8 byte length
 *
 * TODO: implement.
 */
export function measure(_text) {
  throw new Error("TODO: implement measure");
}

/**
 * Truncates to at most `maxBytes` UTF-8 bytes WITHOUT splitting a
 * character: the result must decode back to a prefix of the input.
 *
 * TODO: implement — build up code point by code point and stop before the
 * budget is exceeded.
 */
export function truncateBytes(_text, _maxBytes) {
  throw new Error("TODO: implement truncateBytes");
}

// Exercise 005 — string methods (beginner).
// Goal:   use the modern string surface, and feel that strings are values.
// Drills: trim/split/join, padStart, replaceAll, at(-1), strict-mode
//         assignment to an index.
// Passes: every function returns a NEW string and none of them mutates its
//         input — because none of them can.

/**
 * Trims the ends and collapses every run of inner whitespace to one space.
 * "  Ada   Lovelace \n" -> "Ada Lovelace".
 *
 * TODO: implement. split() with a whitespace pattern does both halves.
 */
export function normalizeSpaces(_text) {
  throw new Error("TODO: implement normalizeSpaces");
}

/** Left-pads a number with zeros to `width`: padId(42, 5) -> "00042". */
export function padId(_id, _width) {
  throw new Error("TODO: implement padId");
}

/**
 * Replaces EVERY occurrence of `needle` with `replacement`.
 * maskTail("a-b-c", "-", "_") -> "a_b_c".
 *
 * TODO: implement. Note that `replace` with a plain string replaces only the
 * first match — there is a method for the other thing.
 */
export function maskTail(_text, _needle, _replacement) {
  throw new Error("TODO: implement maskTail");
}

/**
 * Returns the last character, or undefined for an empty string. Must work
 * with a negative index rather than length arithmetic.
 *
 * TODO: implement.
 */
export function lastChar(_text) {
  throw new Error("TODO: implement lastChar");
}

/**
 * Tries to overwrite the first character in place (`text[0] = "X"`) and
 * returns the string afterwards. A module is strict-mode code, so the
 * assignment is not merely ignored — decide what to do about that.
 *
 * TODO: implement. The returned string is the unchanged original.
 */
export function mutateFirstChar(_text) {
  throw new Error("TODO: implement mutateFirstChar");
}

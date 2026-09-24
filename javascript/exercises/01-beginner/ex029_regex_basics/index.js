// Exercise 029 — regular expressions (beginner).
// Goal:   the four methods, and the flags that change them.
// Drills: test with anchors, match and its capture groups, replace with /g,
//         a case-insensitive pattern, counting matches.
// Passes: isHexColor() rejects a colour with trailing junk (so it is
//         anchored), and maskDigits() replaces every digit, not the first.

/**
 * True for "#fff" and "#a1b2c3", false for anything with extra characters
 * before or after, and false for a wrong length. Case does not matter.
 *
 * TODO: implement with an anchored pattern and `test`.
 */
export function isHexColor(_text) {
  throw new Error("TODO: implement isHexColor");
}

/**
 * The first integer in the text as a NUMBER, or null when there is none.
 * "order 42 of 99" -> 42
 *
 * TODO: implement with `match` and a capture group.
 */
export function firstNumber(_text) {
  throw new Error("TODO: implement firstNumber");
}

/** Replaces every digit with "x". TODO: implement. */
export function maskDigits(_text) {
  throw new Error("TODO: implement maskDigits");
}

/**
 * How many times `word` occurs as a whole word, ignoring case.
 * countWord("Cat cats CAT", "cat") -> 2   ("cats" is a different word)
 *
 * TODO: implement — build the pattern with the RegExp constructor, since
 * the word only exists at runtime, and remember \b.
 */
export function countWord(_text, _word) {
  throw new Error("TODO: implement countWord");
}

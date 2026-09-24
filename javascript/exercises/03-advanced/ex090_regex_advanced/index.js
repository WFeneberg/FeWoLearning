// Exercise 090 — regular expressions, the sharp end (advanced).
// Goal:   lookaround, the sticky flag, unicode property escapes, and a
//         replacement computed per match.
// Drills: (?<=…) and (?<!…), /y and lastIndex, \p{…} with /u, and a
//         replacer function reading the named groups.
// Passes: tokenize() consumes the input strictly left to right — a sticky
//         pattern refuses to skip over anything it cannot match.

/**
 * Every number in the text that is preceded by a currency symbol, as
 * strings — "$12 and 34 and €5" -> ["12", "5"].
 *
 * TODO: implement with a lookbehind.
 */
export function pricesOnly(_text) {
  throw new Error("TODO: implement pricesOnly");
}

/**
 * Every word NOT preceded by "no ".
 * "no cats and dogs" -> ["no", "and", "dogs"]  (only "cats" is excluded)
 *
 * TODO: implement with a negative lookbehind.
 */
export function wordsNotAfterNo(_text) {
  throw new Error("TODO: implement wordsNotAfterNo");
}

/**
 * Splits "12+7-3" into tokens using a STICKY pattern, returning
 * [{ type, value }] where type is "number" or "operator".
 *
 * A sticky regex only matches at lastIndex, so anything unexpected stops
 * the scan: throw a SyntaxError(`unexpected character at <index>`).
 *
 * TODO: implement with /y and a loop over lastIndex.
 */
export function tokenize(_text) {
  throw new Error("TODO: implement tokenize");
}

/**
 * Counts the LETTERS in a string across every script — "Grüße 日本語 123"
 * has 8: five Latin letters and three Han characters.
 *
 * TODO: implement with \p{L} and the /u flag.
 */
export function countLetters(_text) {
  throw new Error("TODO: implement countLetters");
}

/**
 * Replaces every "{name}" placeholder with values[name], leaving an
 * unknown placeholder exactly as it was.
 *
 * TODO: implement with a named group and a replacer FUNCTION.
 */
export function interpolate(_template, _values) {
  throw new Error("TODO: implement interpolate");
}

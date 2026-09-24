// Exercise 030 — named groups and the /g flag's memory (beginner).
// Goal:   read captures by name, and know why a shared /g regex lies.
// Drills: (?<name>…), match.groups, matchAll, $<name> in a replacement,
//         lastIndex.
// Passes: parseLogLine() returns named fields, and staleLastIndex()
//         demonstrates the bug that costs an afternoon.

/**
 * "2024-01-31 ERROR disk full" ->
 *   { date: "2024-01-31", level: "ERROR", message: "disk full" }
 * and null for a line that does not match.
 *
 * TODO: implement with named capture groups and match.groups.
 */
export function parseLogLine(_line) {
  throw new Error("TODO: implement parseLogLine");
}

/**
 * Every "key=value" pair in the text, as an array of { key, value }, in
 * order.
 *
 * TODO: implement with matchAll — which needs the /g flag, and does not
 * suffer from lastIndex because it starts fresh.
 */
export function parsePairs(_text) {
  throw new Error("TODO: implement parsePairs");
}

/**
 * Rewrites every "DD.MM.YYYY" as "YYYY-MM-DD".
 *
 * TODO: implement with named groups and `$<name>` in the replacement
 * string — no callback.
 */
export function reformatDates(_text) {
  throw new Error("TODO: implement reformatDates");
}

/**
 * Creates ONE regex with the /g flag and calls `.test("a1")` on it twice,
 * returning both answers as [first, second].
 *
 * The answer is [true, false]: a /g regex remembers where it stopped in
 * `lastIndex`, so the second call starts past the match and fails. This is
 * why a module-level `const RE = /…/g` shared between calls is a bug.
 *
 * TODO: implement.
 */
export function staleLastIndex() {
  throw new Error("TODO: implement staleLastIndex");
}

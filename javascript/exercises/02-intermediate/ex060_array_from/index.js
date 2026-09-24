// Exercise 060 — building arrays (intermediate).
// Goal:   stop writing `new Array(n).fill()` incantations you do not trust.
// Drills: Array.from with an iterable and a map function, Array.from over
//         an array-LIKE, Array.of, and the holes that make map() skip.
// Passes: sequence() builds [0, 1, 2] with no intermediate fill, and
//         holesVsUndefined() shows why `new Array(3).map(…)` does nothing.

/**
 * [0, 1, …, count-1].
 *
 * TODO: implement with Array.from and its second argument — no fill().
 */
export function sequence(_count) {
  throw new Error("TODO: implement sequence");
}

/**
 * Turns an array-like ({ 0: …, length: n }) into a real array.
 *
 * TODO: implement.
 */
export function fromArrayLike(_arrayLike) {
  throw new Error("TODO: implement fromArrayLike");
}

/**
 * The code point of each character of a string, as numbers.
 *
 * TODO: implement with Array.from and a map function.
 */
export function codePoints(_text) {
  throw new Error("TODO: implement codePoints");
}

/**
 * Wraps its arguments in an array — including a single number, where
 * Array(3) would build a length-3 array of holes instead.
 *
 * TODO: implement with Array.of.
 */
export function arrayOf(..._values) {
  throw new Error("TODO: implement arrayOf");
}

/**
 * Returns { holes, filled } where
 *   - holes is `new Array(3)` mapped with () => 1
 *   - filled is `Array.from({ length: 3 })` mapped with () => 1
 *
 * The first is still three holes, because map() skips them. The second is
 * [1, 1, 1], because Array.from materialises three undefineds.
 *
 * TODO: implement.
 */
export function holesVsUndefined() {
  throw new Error("TODO: implement holesVsUndefined");
}

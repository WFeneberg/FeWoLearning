// Exercise 012 — destructuring (beginner).
// Goal:   take things apart in the parameter list instead of the body.
// Drills: array and object patterns, renaming, defaults, nested patterns,
//         the `= {}` that makes an options parameter optional.
// Passes: every function destructures — and parseOptions() can be called
//         with no arguments at all.

/**
 * Returns { first, rest } where rest is an array of everything else.
 * firstAndRest([]) -> { first: undefined, rest: [] }
 *
 * TODO: implement with an array pattern.
 */
export function firstAndRest(_list) {
  throw new Error("TODO: implement firstAndRest");
}

/**
 * [a, b] -> [b, a], as a new array.
 *
 * TODO: implement with a destructuring assignment — no temp variable.
 */
export function swap(_pair) {
  throw new Error("TODO: implement swap");
}

/**
 * Renames while unpacking, and fills in missing coordinates:
 * pickCoords({ x: 3 }) -> { left: 3, top: 0 }
 *
 * TODO: implement with `{ x: left = 0, y: top = 0 }`.
 */
export function pickCoords(_point) {
  throw new Error("TODO: implement pickCoords");
}

/**
 * parseOptions() -> { retries: 3, label: "job" }
 * parseOptions({ tag: "sync" }) -> { retries: 3, label: "sync" }
 *
 * The parameter is an object with an optional `retries` and an optional
 * `tag`, which is renamed to `label` on the way out. Calling it with no
 * argument at all must work.
 *
 * TODO: implement with a destructuring parameter and a default of {}.
 */
export function parseOptions(_options) {
  throw new Error("TODO: implement parseOptions");
}

/**
 * Digs the first item's title out of
 *   { data: { items: [{ title }] } }
 * and returns "untitled" when any level is missing or the list is empty.
 *
 * TODO: implement with one nested pattern and defaults.
 */
export function firstTitle(_response) {
  throw new Error("TODO: implement firstTitle");
}

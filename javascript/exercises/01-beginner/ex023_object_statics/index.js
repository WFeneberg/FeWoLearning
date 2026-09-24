// Exercise 023 — the Object statics (beginner).
// Goal:   transform objects through entries instead of a for..in loop.
// Drills: keys/values/entries/fromEntries, Object.assign's return value,
//         Object.hasOwn.
// Passes: mapValues() and invert() go through entries, and assignInto()
//         returns the very target it was handed.

/**
 * { a: 1, b: 2 } with fn = n => n * 10  ->  { a: 10, b: 20 }.
 * Key order is preserved. The input is not modified.
 *
 * TODO: implement with entries + fromEntries.
 */
export function mapValues(_object, _fn) {
  throw new Error("TODO: implement mapValues");
}

/**
 * Swaps keys and values: { a: "x" } -> { x: "a" }. Later duplicates win.
 * Values are used as keys, so they become strings.
 *
 * TODO: implement.
 */
export function invert(_object) {
  throw new Error("TODO: implement invert");
}

/**
 * A NEW object with only the requested keys — and only those the object
 * actually has as its OWN properties, so "toString" picks up nothing.
 *
 * TODO: implement with Object.hasOwn.
 */
export function pick(_object, _keys) {
  throw new Error("TODO: implement pick");
}

/**
 * Copies every source's own enumerable properties onto `target`, in order,
 * and returns `target` itself.
 *
 * TODO: implement with Object.assign.
 */
export function assignInto(_target, ..._sources) {
  throw new Error("TODO: implement assignInto");
}

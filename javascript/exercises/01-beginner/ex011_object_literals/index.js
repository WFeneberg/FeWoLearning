// Exercise 011 — object literals (beginner).
// Goal:   write the modern literal forms, and learn the key-order rule.
// Drills: property shorthand, computed keys, nested literals, the order
//         Object.keys reports.
// Passes: tag() builds its key at runtime, and orderedKeys() reports what
//         the runtime really does — integer-like keys first, ascending.

/**
 * { name, age } from two parameters, plus a nested `meta: { source }`.
 *
 * TODO: implement using property shorthand for name and age.
 */
export function makeUser(_name, _age, _source) {
  throw new Error("TODO: implement makeUser");
}

/**
 * A one-property object whose key is built at runtime:
 * tag("env", "prod", 1) -> { "env:prod": 1 }
 *
 * TODO: implement with a computed key — no assignment afterwards.
 */
export function tag(_prefix, _id, _value) {
  throw new Error("TODO: implement tag");
}

/**
 * The object's own enumerable string keys, in the order the runtime lists
 * them. There is nothing to sort here: the spec's order is integer-like
 * keys first in ascending numeric order, then the remaining string keys in
 * insertion order.
 *
 * TODO: implement.
 */
export function orderedKeys(_object) {
  throw new Error("TODO: implement orderedKeys");
}

/**
 * A nested config literal:
 *   { env, server: { port, host: "localhost" }, features: [] }
 * Each call must return a fresh `features` array.
 *
 * TODO: implement.
 */
export function makeConfig(_env, _port) {
  throw new Error("TODO: implement makeConfig");
}

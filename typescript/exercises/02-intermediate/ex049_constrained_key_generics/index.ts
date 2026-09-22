// Exercise 049 — key-constrained generics and where inference comes from
// (intermediate).
// Goal:   write helpers that work over any object and still check the key
//         and the value against each other.
// Drills: two type parameters where one constrains the other, T[K] as a
//         return and as a parameter, inference from an array element.
// Passes: each helper reports the right type per key, and rejects a key or
//         a value that does not belong.
//
// ex018 did this for one fixed type. The general shape is two parameters,
// `<T, K extends keyof T>`, and the payoff is that BOTH get inferred from
// the call: T from the object, K from the key literal. Nothing is written
// at the call site.
//
// The interesting one is setIn, where `T[K]` appears in a PARAMETER
// position. That is what makes the value check against the key that was
// passed, rather than against the union of every value type — so
// `setIn(row, "count", "nope")` is an error while
// `setIn(row, "label", "ok")` is not.
//
// As elsewhere, the stubs take and return `unknown` so the tests compile
// against them; introducing the parameters is the work.

/** TODO: read a property, reporting that property's own type. */
export function getIn(_obj: unknown, _key: unknown): unknown {
  throw new Error("TODO: implement getIn");
}

/** TODO: a copy of `obj` with one property replaced. The value must match
 *  that key's type, not the union of all of them. */
export function setIn(_obj: unknown, _key: unknown, _value: unknown): unknown {
  throw new Error("TODO: implement setIn");
}

/** TODO: one property out of every element. */
export function pluck(_items: unknown, _key: unknown): unknown {
  throw new Error("TODO: implement pluck");
}

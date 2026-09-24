// Exercise 058 — grouping (intermediate).
// Goal:   the 2024 built-ins, and the reduce they replace.
// Drills: Object.groupBy, Map.groupBy, the same thing with reduce, and the
//         null prototype Object.groupBy hands back.
// Passes: groupByType() returns an object with NO prototype — which is the
//         detail that breaks a naive equality assertion.

/**
 * Groups items by `keyFn` into a plain-ish object.
 *
 * TODO: implement with Object.groupBy. Note that the result has a null
 * prototype: no toString, no hasOwnProperty, and a key called "constructor"
 * cannot collide with anything.
 */
export function groupByKey(_items, _keyFn) {
  throw new Error("TODO: implement groupByKey");
}

/**
 * The same grouping as a Map, which can therefore use non-string keys.
 *
 * TODO: implement with Map.groupBy.
 */
export function groupToMap(_items, _keyFn) {
  throw new Error("TODO: implement groupToMap");
}

/**
 * The same grouping again, written by hand with reduce into a Map — what
 * everybody wrote before 2024.
 *
 * TODO: implement with reduce. No Map.groupBy here.
 */
export function groupWithReduce(_items, _keyFn) {
  throw new Error("TODO: implement groupWithReduce");
}

/**
 * Counts items per key: { admin: 2, user: 1 } as a plain Map.
 *
 * TODO: implement.
 */
export function countByKey(_items, _keyFn) {
  throw new Error("TODO: implement countByKey");
}

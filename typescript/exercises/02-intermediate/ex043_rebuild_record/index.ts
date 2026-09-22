// Exercise 043 — rebuilding Record, and what its key type decides
// (intermediate).
// Goal:   map a set of keys onto one value type, and see when that stops
//         being a fixed set.
// Drills: a mapped type over a key union, the PropertyKey constraint, the
//         point at which a mapped type becomes an index signature.
// Passes: MyRecord names exact keys for a literal union and produces an
//         INDEX SIGNATURE for `string`, rejects a key type that cannot be
//         a key, and fromKeys builds one at runtime.
//
// Record is `{ [P in K]: T }` with `K extends keyof any`. The interesting
// part is what happens as K widens.
//
// Given a union of literals, you get exactly those properties, and reading
// a missing one is an error. Given `string`, the same mapped type produces
// `{ [x: string]: V }` — an index signature, where every read succeeds and,
// under this track's noUncheckedIndexedAccess, every read is `V | undefined`.
// Same syntax, very different contract, and the difference is invisible
// until someone widens a key type upstream.
//
// The key type has to be something that can BE a key: string, number or
// symbol, which the standard library spells `keyof any` and everyone else
// spells `PropertyKey`. The stub leaves K unconstrained; fixing that is
// part of the work.

/** TODO: an object with one property per member of K, each valued V.
 *  Constrain K so a type that cannot be a key is rejected. */
export type MyRecord<K, V> = unknown;

/** TODO: an object mapping each key to `value`. */
export function fromKeys<K extends string>(
  _keys: readonly K[],
  _value: number,
): MyRecord<K, number> {
  throw new Error("TODO: implement fromKeys");
}

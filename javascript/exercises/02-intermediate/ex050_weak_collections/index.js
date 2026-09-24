// Exercise 050 — WeakMap, WeakSet, WeakRef (intermediate).
// Goal:   attach data to an object without owning it.
// Drills: a WeakMap as a private-field store, a WeakSet as a marker, a
//         WeakRef's deref(), and the rule that a weak key must be an object.
// Passes: two instances of the same class keep separate private data, and
//         the store exposes no way to enumerate what it holds.
//
// Why weak: the entry disappears when the key is collected, so a cache or
// a side table cannot keep a whole object graph alive. These tests do NOT
// assert collection itself — forcing a GC is not reliable enough to grade.

/**
 * Returns { set, get, has } over a WeakMap keyed by object.
 *
 * TODO: implement — the map must stay captured in the closure, with no
 * property on the returned object holding it.
 */
export function makeSideTable() {
  throw new Error("TODO: implement makeSideTable");
}

/**
 * Returns { mark, wasMarked } over a WeakSet — a one-bit tag per object,
 * with no field on the object itself.
 *
 * TODO: implement.
 */
export function makeMarker() {
  throw new Error("TODO: implement makeMarker");
}

/**
 * Tries to use `key` in a WeakMap and returns "ok" or the thrown error's
 * name. Only objects and non-registered symbols may be weak keys; a string
 * or a number is a TypeError.
 *
 * TODO: implement.
 */
export function useAsWeakKey(_key) {
  throw new Error("TODO: implement useAsWeakKey");
}

/**
 * Wraps `object` in a WeakRef and returns { get } where get() returns
 * deref() — the object while it is still alive, undefined once it is gone.
 *
 * TODO: implement.
 */
export function makeWeakHolder(_object) {
  throw new Error("TODO: implement makeWeakHolder");
}

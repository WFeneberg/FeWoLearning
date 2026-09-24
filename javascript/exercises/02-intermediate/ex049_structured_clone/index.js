// Exercise 049 — structuredClone (intermediate).
// Goal:   deep-copy with the platform instead of JSON.
// Drills: structuredClone, cycles, Map/Set/Date/RegExp/BigInt, what it
//         refuses (functions, symbols) and what it silently flattens
//         (prototypes, getters).
// Passes: cloneWithCycle() returns a copy whose self-reference points at
//         the COPY, and cloneFunction() reports a DataCloneError.

/** A deep copy. TODO: implement with structuredClone. */
export function clone(_value) {
  throw new Error("TODO: implement clone");
}

/**
 * Builds { name: "self", self: <itself> }, clones it, and returns the
 * clone. JSON.stringify would throw a TypeError on this.
 *
 * TODO: implement.
 */
export function cloneWithCycle() {
  throw new Error("TODO: implement cloneWithCycle");
}

/**
 * Tries to clone a value that cannot be cloned and returns the caught
 * error's name — "DataCloneError".
 *
 * TODO: implement.
 */
export function cloneUncloneable(_value) {
  throw new Error("TODO: implement cloneUncloneable");
}

/**
 * Clones an instance of `class Tagged { constructor(value) }` carrying a
 * `describe()` method, and returns
 *   { isTagged, hasDescribe, value }
 * read off the CLONE.
 *
 * structuredClone copies data, not behaviour: the clone is a plain object
 * with the same own properties, so isTagged and hasDescribe are both false.
 *
 * TODO: implement — define the class inside the function.
 */
export function cloneLosesPrototype() {
  throw new Error("TODO: implement cloneLosesPrototype");
}

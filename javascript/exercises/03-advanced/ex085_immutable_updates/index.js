// Exercise 085 — immutable updates with structural sharing (advanced).
// Goal:   change one leaf without copying the whole tree.
// Drills: copying along a path, leaving siblings alone, arrays vs objects,
//         and the identity checks that prove the sharing happened.
// Passes: an untouched branch comes back as the SAME reference, which is
//         what makes a `prev !== next` change check work at all.

/**
 * A copy of `object` with `path` (an array of keys) set to `value`.
 *   - every object on the path is copied
 *   - every branch off the path keeps its identity
 *   - a numeric key into an array produces an array, not an object
 *   - a missing intermediate object is created (as an object)
 *
 * TODO: implement recursively.
 */
export function setIn(_object, _path, _value) {
  throw new Error("TODO: implement setIn");
}

/**
 * The same, with the new value computed from the old one by `fn`.
 *
 * TODO: implement — reuse setIn.
 */
export function updateIn(_object, _path, _fn) {
  throw new Error("TODO: implement updateIn");
}

/**
 * The value at `path`, or `fallback` when any step is missing.
 *
 * TODO: implement.
 */
export function getIn(_object, _path, _fallback) {
  throw new Error("TODO: implement getIn");
}

/**
 * A copy without the property at `path`. An array element is REMOVED
 * (the array gets shorter), an object key is deleted.
 *
 * TODO: implement.
 */
export function removeIn(_object, _path) {
  throw new Error("TODO: implement removeIn");
}

// Exercise 007 — mutating vs copying (beginner).
// Goal:   know which array methods change the array you were given.
// Drills: push/splice (in place) vs slice/spread/toSpliced (copying).
// Passes: the copying functions leave their input untouched and return a
//         different array; drainInto() returns the very object it was given.
//
// Coming from C#: an array here is always passed by reference and always
// growable. `splice` is the in-place editor; the `to*` methods added in 2023
// (toSpliced, toSorted, toReversed, with) are its copying twins.

/** Returns a NEW array with `value` appended. TODO: implement. */
export function appendCopy(_list, _value) {
  throw new Error("TODO: implement appendCopy");
}

/**
 * Returns a NEW array with the element at `index` removed. An out-of-range
 * index removes nothing.
 *
 * TODO: implement.
 */
export function removeAt(_list, _index) {
  throw new Error("TODO: implement removeAt");
}

/**
 * Returns a NEW array with `value` inserted before `index`.
 *
 * TODO: implement.
 */
export function insertAt(_list, _index, _value) {
  throw new Error("TODO: implement insertAt");
}

/**
 * Appends every element of `source` to `target` IN PLACE and returns
 * `target` itself. `source` is not modified.
 *
 * TODO: implement. Watch the argument count — a million-element source
 * spread into push() will overflow the call stack, so prefer a loop or
 * chunking if you care about that.
 */
export function drainInto(_target, _source) {
  throw new Error("TODO: implement drainInto");
}

/**
 * Returns a NEW array holding at most the first `count` elements.
 *
 * TODO: implement.
 */
export function takeFirst(_list, _count) {
  throw new Error("TODO: implement takeFirst");
}

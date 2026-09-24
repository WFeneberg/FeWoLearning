// Exercise 056 — deep equality (intermediate).
// Goal:   write the comparison every test framework ships.
// Drills: recursion over arrays and plain objects, NaN, Date, key-count
//         checks, and a seen-pairs set so a cycle terminates.
// Passes: two self-referencing objects compare equal instead of blowing
//         the stack.

/**
 * Structural equality for primitives, arrays, plain objects and Dates.
 *
 *   - primitives compare with SameValueZero, so NaN equals NaN
 *   - arrays compare element-wise, length first
 *   - plain objects compare own enumerable keys, count first
 *   - Dates compare by time value
 *   - an array is never equal to an object, even with the same keys
 *   - cycles must not hang: a pair already being compared counts as equal
 *
 * TODO: implement.
 */
export function deepEqual(_a, _b) {
  throw new Error("TODO: implement deepEqual");
}

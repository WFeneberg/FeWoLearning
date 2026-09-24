// Exercise 032 — the iterable protocol (beginner).
// Goal:   make your own object work with for..of, spread and destructuring.
// Drills: Symbol.iterator, the { value, done } iterator contract, why the
//         method must hand out a FRESH iterator each time.
// Passes: a range can be walked twice with the same result, and everything
//         that consumes iterables accepts it.

/**
 * Returns an object that iterates start, start+step, … while below `end`
 * (end is exclusive). It must be re-iterable: two for..of loops over the
 * same object both see every value.
 *
 * Write the iterator by hand — `function*` is the next exercise.
 *
 * TODO: implement with [Symbol.iterator]() returning { next() }.
 */
export function makeRange(_start, _end, _step = 1) {
  throw new Error("TODO: implement makeRange");
}

/**
 * True when `value` can be used with for..of — i.e. it has a callable
 * Symbol.iterator — without iterating it.
 *
 * TODO: implement. Must not throw for null or a number.
 */
export function isIterable(_value) {
  throw new Error("TODO: implement isIterable");
}

/**
 * Pulls exactly `count` values out of an iterable by using its iterator
 * DIRECTLY — no for..of, no spread — and returns them. Stops early if the
 * iterator finishes first.
 *
 * TODO: implement with .next() and the done flag.
 */
export function pullValues(_iterable, _count) {
  throw new Error("TODO: implement pullValues");
}

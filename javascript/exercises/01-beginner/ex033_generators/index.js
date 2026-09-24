// Exercise 033 — generators (beginner).
// Goal:   write an iterator without writing an iterator.
// Drills: function*, yield, laziness, an endless sequence, consuming one
//         generator from another.
// Passes: naturals() never finishes and never hangs the test, and
//         countedRange() proves the body does not run until the first pull.

/**
 * Yields start, start+step, … while below `end` (exclusive).
 *
 * TODO: implement as a generator.
 */
export function* range(_start, _end, _step = 1) {
  throw new Error("TODO: implement range");
}

/**
 * Yields 0, 1, 2, … forever. Safe, because nothing runs until it is pulled.
 *
 * TODO: implement.
 */
export function* naturals() {
  throw new Error("TODO: implement naturals");
}

/**
 * Yields at most the first `count` values of ANY iterable, including an
 * endless one.
 *
 * TODO: implement as a generator. Do not materialise the source.
 */
export function* take(_iterable, _count) {
  throw new Error("TODO: implement take");
}

/**
 * Calls `onEnter()` as the FIRST statement of the generator body, then
 * yields 1 and 2.
 *
 * Calling a generator function runs none of its body — the callback must
 * not fire until the first next(). That is the whole point of this one.
 *
 * TODO: implement.
 */
export function* countedRange(_onEnter) {
  throw new Error("TODO: implement countedRange");
}

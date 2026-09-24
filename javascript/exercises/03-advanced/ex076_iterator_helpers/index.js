// Exercise 076 — iterator helpers (advanced).
// Goal:   array-shaped operations that never build the array.
// Drills: Iterator.prototype map/filter/take/drop/flatMap/reduce/toArray,
//         Iterator.from, and laziness over an endless source.
// Passes: firstMatching() pulls exactly as far as it must — the tests
//         count the pulls, so an eager `[...source]` implementation fails
//         even where it would produce the right answer.

/**
 * The first `count` values of an iterator that satisfy `predicate`, as an
 * array. The source may be endless.
 *
 * TODO: implement with the iterator helpers — filter().take().toArray().
 */
export function firstMatching(_iterator, _predicate, _count) {
  throw new Error("TODO: implement firstMatching");
}

/**
 * Sums the squares of the values an iterator yields, without building an
 * intermediate array.
 *
 * TODO: implement with map() and reduce().
 */
export function sumOfSquares(_iterator) {
  throw new Error("TODO: implement sumOfSquares");
}

/**
 * Skips `offset` values and then takes `limit` — a paging window over any
 * iterator.
 *
 * TODO: implement with drop() and take().
 */
export function page(_iterator, _offset, _limit) {
  throw new Error("TODO: implement page");
}

/**
 * Turns an ITERABLE (an array, a Set, a string) into a helper-capable
 * iterator and maps it, returning the array.
 *
 * TODO: implement with Iterator.from — a plain array's own .values() would
 * also work, but the point is the adapter.
 */
export function mapIterable(_iterable, _fn) {
  throw new Error("TODO: implement mapIterable");
}

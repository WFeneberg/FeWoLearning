// Exercise 061 — writing an iterator by hand (intermediate).
// Goal:   the protocol a generator implements for you.
// Drills: { next }, the done flag, the OPTIONAL return() that runs on an
//         early exit, and composing iterables without a generator.
// Passes: breaking out of a for..of over closeable() runs its cleanup —
//         because for..of calls return() on the way out.

/**
 * An iterable counting from `from` down to 1.
 *
 * TODO: implement with [Symbol.iterator]() returning a hand-written
 * iterator object. No function*.
 */
export function countdown(_from) {
  throw new Error("TODO: implement countdown");
}

/**
 * An iterable over `values` whose iterator has a `return()` calling
 * `onClose()` exactly once.
 *
 * for..of calls return() when the loop is left early (break, throw, or a
 * destructuring that stops), and NOT when the iterator finished on its own.
 *
 * TODO: implement.
 */
export function closeable(_values, _onClose) {
  throw new Error("TODO: implement closeable");
}

/**
 * One iterable running through every given iterable in order.
 * chain([1, 2], "ab") -> 1, 2, "a", "b"
 *
 * TODO: implement with hand-written iterators — no function*, no spread of
 * the sources (it must stay lazy).
 */
export function chain(..._iterables) {
  throw new Error("TODO: implement chain");
}

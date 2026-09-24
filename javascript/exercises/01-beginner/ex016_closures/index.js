// Exercise 016 — closures (beginner).
// Goal:   keep state in a scope instead of on an object.
// Drills: a factory returning functions over private state, one-shot
//         memory, per-iteration bindings in a loop.
// Passes: two counters never interfere, once() forgets nothing and calls
//         nothing twice, and indexReaders() does NOT return N copies of N.

/**
 * Returns { next, reset } over a private counter starting at `start`.
 * next() returns the current value and then advances by one; reset() puts
 * it back to `start`. The count must not be reachable as a property of the
 * returned object.
 *
 * TODO: implement.
 */
export function makeCounter(_start = 0) {
  throw new Error("TODO: implement makeCounter");
}

/**
 * Wraps `fn` so it runs at most once. Later calls return the first result
 * without calling `fn` again — including when the first result was
 * undefined.
 *
 * TODO: implement.
 */
export function once(_fn) {
  throw new Error("TODO: implement once");
}

/**
 * An array of `count` functions; the i-th returns i.
 *
 * TODO: implement. With `var` as the loop variable every function would
 * return `count`, because they would all close over the ONE binding. `let`
 * makes a fresh binding per iteration — which is the whole exercise.
 */
export function indexReaders(_count) {
  throw new Error("TODO: implement indexReaders");
}

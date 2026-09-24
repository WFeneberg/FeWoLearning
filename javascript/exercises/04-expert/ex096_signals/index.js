// Exercise 096 — signals (expert).
// Goal:   automatic dependency tracking, in about forty lines.
// Drills: a current-listener stack, subscribing on READ, lazy computed
//         values with caching, and batching so one change notifies once.
// Passes: a computed value recomputes only when something it actually
//         READ has changed — the tests count the recomputations, so a
//         version that recomputes on every access fails.

/**
 * A readable/writable value: signal(initial) -> { get, set, peek }
 *   get()  reads it AND registers it as a dependency of whatever is
 *          currently being computed
 *   set(v) writes it and notifies the dependents, unless the value is
 *          unchanged (SameValueZero)
 *   peek() reads without registering anything
 *
 * TODO: implement.
 */
export function signal(_initial) {
  throw new Error("TODO: implement signal");
}

/**
 * A derived value: computed(fn) -> { get, peek }
 *   - fn runs lazily, on the first get()
 *   - the result is cached until one of the signals fn READ changes
 *   - a computed may read other computeds
 *   - reading a computed inside another computed makes it a dependency too
 *
 * TODO: implement.
 */
export function computed(_fn) {
  throw new Error("TODO: implement computed");
}

/**
 * Runs `fn` immediately, tracking what it reads, and re-runs it whenever
 * one of those changes. Returns a function that stops it.
 *
 * TODO: implement.
 */
export function effect(_fn) {
  throw new Error("TODO: implement effect");
}

/**
 * Runs `fn` with notifications deferred: every effect that would have run
 * runs ONCE at the end, however many signals were written.
 *
 * TODO: implement.
 */
export function batch(_fn) {
  throw new Error("TODO: implement batch");
}

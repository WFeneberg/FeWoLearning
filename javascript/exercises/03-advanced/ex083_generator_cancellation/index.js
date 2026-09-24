// Exercise 083 — cancelling a generator (advanced).
// Goal:   stop a producer from the outside, and let it clean up.
// Drills: gen.return(), gen.throw(), try/finally inside a generator, a
//         generator that refuses to stop, and what for..of does on break.
// Passes: the producer's finally block runs on return(), and a generator
//         that catches the injected error keeps going.

/**
 * Yields from `values` and pushes "cleanup" onto `log` in a finally block.
 *
 * TODO: implement.
 */
export function* cleaningProducer(_values, _log) {
  throw new Error("TODO: implement cleaningProducer");
}

/**
 * Pulls `count` values out of a generator and then calls return(),
 * returning { values, result } where `result` is what return() gave back.
 *
 * TODO: implement.
 */
export function pullThenReturn(_generator, _count) {
  throw new Error("TODO: implement pullThenReturn");
}

/**
 * Yields 1, 2, 3 …, catching anything thrown INTO it and yielding
 * `caught: <message>` instead of dying. Endless.
 *
 * TODO: implement — a try/catch around the yield, inside a loop.
 */
export function* resilient() {
  throw new Error("TODO: implement resilient");
}

/**
 * Calls gen.throw(error) and returns "propagated" if the error came back
 * out, or { yielded } with whatever the generator yielded instead.
 *
 * TODO: implement.
 */
export function throwInto(_generator, _error) {
  throw new Error("TODO: implement throwInto");
}

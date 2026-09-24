// Exercise 099 — SharedArrayBuffer and Atomics (expert).
// Goal:   shared memory, and the operations that make it safe.
// Drills: SharedArrayBuffer, Int32Array over it, Atomics.load/store/add,
//         compareExchange, and a single-producer single-consumer ring.
// Passes: the ring reports full and empty correctly at the wrap-around,
//         which is where every hand-written ring buffer is wrong.
//
// Graded single-threaded on purpose: Atomics.wait is forbidden on the
// main thread, and a race is not something a test can assert on. ex098 is
// the row that actually starts a thread.

/**
 * A ring buffer over a SharedArrayBuffer holding `capacity` int32 values:
 *   push(value) -> true, or false when it is full
 *   shift()     -> the oldest value, or undefined when empty
 *   size, capacity
 *
 * Layout: two int32 control slots (read index, write index) followed by
 * `capacity` data slots. Use Atomics for every read and write of the
 * control slots — that is what makes the structure shareable at all.
 *
 * TODO: implement. One slot is deliberately left unused, or a full ring
 * is indistinguishable from an empty one.
 */
export function createRing(_capacity) {
  throw new Error("TODO: implement createRing");
}

/**
 * Increments a shared counter `times` times with Atomics.add and returns
 * the final value read back with Atomics.load.
 *
 * TODO: implement over a SharedArrayBuffer.
 */
export function countWithAtomics(_times) {
  throw new Error("TODO: implement countWithAtomics");
}

/**
 * A compare-and-swap attempt: writes `next` into slot 0 only if it still
 * holds `expected`, and returns { previous, written }.
 *
 * TODO: implement with Atomics.compareExchange over the given Int32Array.
 */
export function tryCompareExchange(_view, _expected, _next) {
  throw new Error("TODO: implement tryCompareExchange");
}

// Exercise 039 — the job queue (intermediate).
// Goal:   know what runs before what, without guessing.
// Drills: synchronous code, the microtask queue (promises, queueMicrotask),
//         the macrotask queue (setTimeout), and the rule that the microtask
//         queue is drained EMPTY before any timer runs.
// Passes: recordOrder() returns the one true order, and
//         microtaskStarvation() shows a microtask chain postponing a timer
//         that was already due.

/**
 * Schedules, in this source order:
 *   1. a setTimeout(…, 0) pushing "timeout"
 *   2. a promise callback pushing "promise"
 *   3. a queueMicrotask pushing "microtask"
 *   4. a synchronous push of "sync"
 * and resolves to the array once the timeout has run.
 *
 * TODO: implement. Think about the answer before you run it.
 */
export function recordOrder() {
  throw new Error("TODO: implement recordOrder");
}

/**
 * Queues a setTimeout(…, 0) that pushes "timer" onto the log, then a chain
 * of `depth` microtasks, each pushing "micro". Resolves to the log once the
 * timer has fired.
 *
 * Every microtask runs before the timer, however many there are: the queue
 * is drained to empty, and a microtask that queues another microtask
 * extends the same drain.
 *
 * TODO: implement.
 */
export function microtaskStarvation(_depth) {
  throw new Error("TODO: implement microtaskStarvation");
}

/**
 * Resolves after exactly `turns` microtask turns, resolving to `turns`.
 * The building block the other rows use instead of a timer.
 *
 * TODO: implement — one await of an already-resolved promise per turn.
 */
export function afterTurns(_turns) {
  throw new Error("TODO: implement afterTurns");
}

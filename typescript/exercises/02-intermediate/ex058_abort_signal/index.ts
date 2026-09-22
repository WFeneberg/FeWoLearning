// Exercise 058 — cancellation is cooperative (intermediate).
// Goal:   stop work that is already running, by asking it to stop.
// Drills: AbortController and AbortSignal, `aborted` and `reason`,
//         throwIfAborted, listening for the abort event.
// Passes: withAbort loses to an abort, runSequentially stops between
//         tasks, and an already-aborted signal does nothing at all.
//
// AbortSignal is the platform's CancellationToken, and it works the same
// way: nothing is forcibly stopped. The signal is a notification, and code
// that never looks at it runs to completion regardless. Aborting a promise
// is therefore always a race between the work and the signal — the work
// keeps going, you simply stop waiting for it.
//
// Three members carry this row. `signal.aborted` is the flag to check
// between steps; `signal.reason` is whatever was passed to abort(),
// defaulting to a DOMException named "AbortError"; and
// `signal.throwIfAborted()` does both in one call.
//
// For the event-driven half, `signal.addEventListener("abort", …)` fires
// once, and only if the signal was not already aborted when you
// subscribed — so a correct withAbort has to check the flag first as well
// as subscribe. That ordering is what the already-aborted fact grades.
//
// This row is graded by runtime facts only. Both signatures are given in
// full because neither is the subject here, and a type fact over a
// signature the stub already carries is green before any work is done.

/** TODO: resolve with `work`'s value, or reject with the signal's reason
 *  if the signal aborts first — including when it is already aborted. */
export function withAbort<T>(_work: Promise<T>, _signal: AbortSignal): Promise<T> {
  throw new Error("TODO: implement withAbort");
}

/**
 * TODO: run the tasks one after another, collecting their results, and
 * stop before starting the next one once the signal has aborted. Return
 * what was collected. An already-aborted signal runs nothing.
 */
export async function runSequentially<T>(
  _tasks: readonly (() => Promise<T>)[],
  _signal: AbortSignal,
): Promise<T[]> {
  throw new Error("TODO: implement runSequentially");
}

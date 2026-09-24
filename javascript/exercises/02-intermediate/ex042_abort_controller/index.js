// Exercise 042 — AbortController (intermediate).
// Goal:   the platform's standard cancellation token.
// Drills: AbortController/AbortSignal, `aborted`, `reason`,
//         throwIfAborted(), the abort event, AbortSignal.abort(),
//         AbortSignal.any().
// Passes: withAbort() rejects with the signal's own reason — and rejects
//         IMMEDIATELY for a signal that was already aborted.

/**
 * Resolves like `promise`, unless `signal` aborts first — in which case it
 * rejects with `signal.reason`. A signal that is already aborted must
 * reject without waiting for the promise at all.
 *
 * TODO: implement.
 */
export function withAbort(_promise, _signal) {
  throw new Error("TODO: implement withAbort");
}

/**
 * The `reason` of a signal aborted with no argument, and with an explicit
 * one: { defaultName, defaultIsDomException, explicit }.
 *
 * Use the AbortSignal.abort() static for both — it returns an
 * already-aborted signal, which is how a "cancelled before we started"
 * case is spelled.
 *
 * TODO: implement. The default reason is a DOMException named "AbortError".
 */
export function abortReasons(_explicitReason) {
  throw new Error("TODO: implement abortReasons");
}

/**
 * Subscribes `fn` to the signal's abort event, ONCE, and returns a function
 * that unsubscribes. Must call `fn` immediately (synchronously) if the
 * signal is already aborted.
 *
 * TODO: implement.
 */
export function onAbort(_signal, _fn) {
  throw new Error("TODO: implement onAbort");
}

/**
 * Runs `step()` in a loop, awaiting each call, until the signal aborts —
 * checking with throwIfAborted() before each step — and returns how many
 * steps completed. Never throws: an abort ends the loop.
 *
 * TODO: implement.
 */
export async function runUntilAborted(_signal, _step) {
  throw new Error("TODO: implement runUntilAborted");
}

// Exercise 094 — a task group (expert).
// Goal:   no task outlives the call that started it.
// Drills: AbortController per group, propagating an external signal,
//         cancelling siblings on the first failure, and waiting for every
//         task to finish unwinding before returning.
// Passes: when one task fails the others are ABORTED and awaited — the
//         group does not resolve while a sibling is still running, which
//         is the difference from Promise.all.

/**
 * Runs every task with a shared group signal:
 *   runGroup(tasks, { signal }) -> array of results, in input order
 *
 * Each task is called as task(signal).
 *   - the first rejection aborts the group's signal
 *   - the group waits for EVERY task to settle before it rejects
 *   - it rejects with the first error
 *   - an outer `signal` (optional) aborts the group too
 *   - a task's own abort error must not replace the original failure
 *
 * TODO: implement.
 */
export function runGroup(_tasks, _options) {
  throw new Error("TODO: implement runGroup");
}

/**
 * Runs tasks until the FIRST one succeeds, aborts the rest, and resolves
 * with that result — the "first usable answer wins" shape.
 *
 * If every task fails, rejects with an AggregateError. Waits for all of
 * them to unwind either way.
 *
 * TODO: implement.
 */
export function runFirst(_tasks) {
  throw new Error("TODO: implement runFirst");
}

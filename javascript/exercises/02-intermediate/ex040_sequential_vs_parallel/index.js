// Exercise 040 — sequential vs parallel (intermediate).
// Goal:   know which of two three-line loops keeps N requests waiting.
// Drills: await in a loop, starting first and awaiting later, Promise.all,
//         and the failure difference between them.
// Passes: the tests watch how many tasks are in flight at once, so
//         a sequential implementation cannot pass the parallel row.

/**
 * Runs each task function one after another and returns their results in
 * order. Never more than one in flight.
 *
 * TODO: implement.
 */
export async function runSequential(_tasks) {
  throw new Error("TODO: implement runSequential");
}

/**
 * Starts every task immediately and returns their results in input order.
 *
 * TODO: implement with Promise.all.
 */
export async function runParallel(_tasks) {
  throw new Error("TODO: implement runParallel");
}

/**
 * Starts every task immediately, but returns only the ones that succeeded,
 * in input order — a rejection is skipped rather than fatal.
 *
 * TODO: implement.
 */
export async function runParallelIgnoringFailures(_tasks) {
  throw new Error("TODO: implement runParallelIgnoringFailures");
}

/**
 * Starts the two tasks in parallel and returns { first, second } — the
 * classic "start both, then await both" shape, written without
 * Promise.all: call each task, keep the promises, await them afterwards.
 *
 * TODO: implement.
 */
export async function runBoth(_first, _second) {
  throw new Error("TODO: implement runBoth");
}

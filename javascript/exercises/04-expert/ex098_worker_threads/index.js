// Exercise 098 — worker threads (expert).
// Goal:   real parallelism, and the cost of the boundary.
// Drills: node:worker_threads, postMessage/once("message"), terminating,
//         structured-clone semantics across the boundary, transferring an
//         ArrayBuffer, and surfacing a worker's error as a rejection.
// Passes: a transferred buffer is DETACHED on this side afterwards, which
//         is the observable difference between transferring and copying.
//
// ./worker.js is provided. Build its URL from import.meta.url — the `@ex`
// alias is a bundler thing and means nothing to Node at runtime.

/**
 * Starts the worker, sends { type: "sum", values }, resolves with the
 * total, and terminates the worker either way.
 *
 * TODO: implement.
 */
export function sumInWorker(_values) {
  throw new Error("TODO: implement sumInWorker");
}

/**
 * Sends a Uint8Array's buffer to the worker to be filled with `value`,
 * TRANSFERRING it, and resolves with
 *   { filled: <the bytes that came back>, senderDetached: <boolean> }
 * where senderDetached says whether this side's view was detached by the
 * transfer.
 *
 * TODO: implement — postMessage(message, [buffer]) is the transfer list.
 */
export function fillInWorker(_size, _value) {
  throw new Error("TODO: implement fillInWorker");
}

/**
 * Sends { type: "fail", reason } and rejects with the worker's error.
 *
 * TODO: implement — listen for the "error" event, not just "message".
 */
export function failInWorker(_reason) {
  throw new Error("TODO: implement failInWorker");
}

/**
 * Sends a function across the boundary and reports what happens:
 * "DataCloneError" — postMessage uses the structured clone algorithm,
 * exactly like structuredClone (ex049), so a function cannot go.
 *
 * TODO: implement.
 */
export function sendFunction() {
  throw new Error("TODO: implement sendFunction");
}

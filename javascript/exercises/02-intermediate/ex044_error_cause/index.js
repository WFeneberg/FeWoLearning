// Exercise 044 — error chains (intermediate).
// Goal:   add context to a failure without losing the original.
// Drills: the `cause` option, walking a cause chain, AggregateError.
// Passes: wrapErrors() keeps the original reachable, and rootCause()
//         reaches the bottom of a chain of any depth.

/**
 * Runs `fn`. If it throws, throws a new Error(message) whose `cause` is the
 * original. Otherwise returns fn's result.
 *
 * TODO: implement — `new Error(message, { cause })`.
 */
export function wrapErrors(_fn, _message) {
  throw new Error("TODO: implement wrapErrors");
}

/**
 * The deepest `cause` in the chain — the error that has no cause of its
 * own. Returns the error itself when there is no cause.
 *
 * TODO: implement.
 */
export function rootCause(_error) {
  throw new Error("TODO: implement rootCause");
}

/**
 * Every message in the chain, outermost first.
 *
 * TODO: implement.
 */
export function causeMessages(_error) {
  throw new Error("TODO: implement causeMessages");
}

/**
 * Calls every function in `tasks`. If one or more throw, throws an
 * AggregateError with `message` whose `errors` holds them in task order.
 * If none throws, returns the array of results.
 *
 * TODO: implement.
 */
export function runAll(_tasks, _message) {
  throw new Error("TODO: implement runAll");
}

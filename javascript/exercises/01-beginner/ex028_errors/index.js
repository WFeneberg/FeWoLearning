// Exercise 028 — errors (beginner).
// Goal:   throw something a caller can actually branch on.
// Drills: subclassing Error, setting `name`, try/catch/finally, the
//         difference between returning from finally and returning from try.
// Passes: ValidationError is an Error, carries its field, and a caught
//         error can be told apart from a programming mistake.

/**
 * An Error subclass with:
 *   - `name` === "ValidationError"
 *   - a `field` property from the second constructor argument
 *   - the message passed through to Error
 *
 * TODO: implement — call super(message) and set the two properties.
 */
export class ValidationError extends Error {
  constructor(_message, _field) {
    super();
    throw new Error("TODO: implement ValidationError");
  }
}

/**
 * Returns the age when it is an integer from 0 to 149. Otherwise throws a
 * ValidationError with field "age" and message:
 *   - "age must be a number" when it is not a number (NaN counts)
 *   - "age out of range" when it is a number outside 0..149
 *
 * TODO: implement.
 */
export function validateAge(_value) {
  throw new Error("TODO: implement validateAge");
}

/**
 * Runs `fn` and reports what happened, always running the cleanup:
 *   { ok: true, value }                 when it returns
 *   { ok: false, error }                when it throws
 * and in BOTH cases calls `onFinally` exactly once before returning.
 *
 * TODO: implement with try/catch/finally.
 */
export function attempt(_fn, _onFinally) {
  throw new Error("TODO: implement attempt");
}

/**
 * Returns "finally". A `return` inside `finally` replaces the one in `try`
 * — including a return that was already carrying a thrown error away.
 * Return "try" from the try block and "finally" from the finally block.
 *
 * TODO: implement.
 */
export function finallyWins() {
  throw new Error("TODO: implement finallyWins");
}

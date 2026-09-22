// Exercise 028 — subclassing Error, and the cause chain (beginner).
// Goal:   raise an error that carries structured context and the failure
//         underneath it.
// Drills: extending Error, `name`, the `cause` option, walking a chain.
// Passes: a ValidationError is an Error and a ValidationError, names itself,
//         carries its field readonly, and causeChain reads the whole stack
//         of reasons.
//
// `cause` (ES2022) is the standard way to wrap: `new Error(msg, { cause })`
// keeps the original instead of flattening it into a string, which is what
// C#'s InnerException does. The difference is that `cause` is typed
// `unknown`, because JavaScript lets you throw anything — see ex029.
//
// One trap that does not apply here but bites in older setups: subclassing
// built-ins like Error only works when the output targets ES2015 or later.
// Compiled down to ES5, the prototype chain is broken and `instanceof` on
// your subclass returns false. This track targets ESNext, so it works.

export class ValidationError extends Error {
  /** TODO: the offending field name, readonly. */
  field: unknown = undefined;

  /**
   * TODO: pass the message and the cause up to Error, set `name` to
   * "ValidationError", and record the field.
   */
  constructor(_message: string, _field: string, _options?: { cause?: unknown }) {
    super();
    throw new Error("TODO: implement the ValidationError constructor");
  }
}

/**
 * TODO: the message of `error` followed by the message of each cause beneath
 * it, outermost first. Stop when a cause is not an Error.
 */
export function causeChain(_error: Error): string[] {
  throw new Error("TODO: implement causeChain");
}

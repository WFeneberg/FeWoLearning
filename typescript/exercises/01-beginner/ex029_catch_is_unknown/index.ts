// Exercise 029 — the catch binding is `unknown` (beginner).
// Goal:   deal with the fact that JavaScript can throw anything at all.
// Drills: narrowing in a catch, normalising an arbitrary throw into an Error.
// Passes: tryRun reports each kind of failure differently, and toError
//         always produces a real Error without losing the original.
//
// `throw` takes any expression, so a catch binding genuinely cannot be typed
// Error. Under `strict` — which includes useUnknownInCatchVariables — it is
// `unknown`, and every access has to be earned. That is a real difference
// from C#, where the only thing that can be thrown is an Exception, and the
// reason library code normalises at the boundary.
//
// toError carries NO return type annotation, deliberately: the graded type
// is the one your body produces.

/**
 * TODO: run `fn` and return its value. If it throws, return
 *   `error:<message>` when an Error was thrown,
 *   `thrown:<value>`  when a string was thrown,
 *   `unknown`         for anything else.
 */
export function tryRun(_fn: () => string): string {
  throw new Error("TODO: implement tryRun");
}

/**
 * TODO: give back `value` unchanged when it already is an Error. Otherwise
 * wrap it in a new Error whose message is String(value) and whose `cause` is
 * the original value.
 */
export function toError(_value: unknown) {
  throw new Error("TODO: implement toError");
}

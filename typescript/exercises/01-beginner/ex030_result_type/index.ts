// Exercise 030 — a Result type instead of exceptions (beginner).
// Goal:   make failure part of the return type, so the checker forces the
//         caller to deal with it.
// Drills: a generic discriminated union, constructors for each arm,
//         combinators that only touch the success arm.
// Passes: Result is the two-arm union, ok and err build the right arm, and
//         mapResult and unwrapOr behave on both.
//
// A thrown error is invisible in a signature: `parse(s: string): number` says
// nothing about failing, and neither TypeScript nor C# makes you handle it.
// A Result makes it visible and unavoidable — you cannot reach `.value`
// without first narrowing on `.ok`, which is ex015's discriminated union
// doing real work.
//
// The `ok` discriminant is a literal `true`/`false`, not `boolean`. That is
// the whole mechanism: `boolean` would not discriminate, and both arms'
// fields would be errors.
//
// As elsewhere, the stubs are declared with `unknown` so the tests compile
// against them; introducing the type parameters is the exercise.

/** TODO: either a success carrying a `value` of type T, or a failure
 *  carrying an `error` of type E. Discriminate on a boolean literal `ok`. */
export type Result<T, E> = unknown;

/** TODO: a success. */
export function ok(_value: unknown): unknown {
  throw new Error("TODO: implement ok");
}

/** TODO: a failure. */
export function err(_error: unknown): unknown {
  throw new Error("TODO: implement err");
}

/** TODO: apply `fn` to a success's value; pass a failure through untouched. */
export function mapResult(_result: unknown, _fn: (value: never) => unknown): unknown {
  throw new Error("TODO: implement mapResult");
}

/** TODO: the value of a success, or `fallback` for a failure. */
export function unwrapOr(_result: unknown, _fallback: unknown): unknown {
  throw new Error("TODO: implement unwrapOr");
}

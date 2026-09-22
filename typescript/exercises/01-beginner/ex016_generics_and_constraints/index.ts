// Exercise 016 — type parameters and constraints (beginner).
// Goal:   carry a caller's type through a function instead of flattening it.
// Drills: a first type parameter, `extends` as a constraint, inference from
//         the argument.
// Passes: identity gives back the exact type it was handed, longer requires
//         something with a length, and a number argument is rejected.
//
// Both stubs are declared with `unknown` parameters and returns. That is
// deliberate and it is the shape of the exercise: `unknown` accepts every
// call, so the tests compile against the stub, and it throws away what the
// caller knew — which is exactly what the type facts catch. Your job is to
// replace it with a type parameter.
//
// A C# note: `T extends { length: number }` is a STRUCTURAL constraint. There
// is no interface to implement and nothing to declare — a string satisfies it
// because strings happen to have a length.

/** TODO: give back the argument, keeping its exact type. */
export function identity(_value: unknown): unknown {
  throw new Error("TODO: implement identity");
}

/**
 * TODO: the longer of two values, compared by `.length`. Constrain the type
 * parameter so that anything without a length is rejected at compile time.
 * Ties return `a`.
 */
export function longer(_a: unknown, _b: unknown): unknown {
  throw new Error("TODO: implement longer");
}

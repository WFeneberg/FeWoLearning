// Exercise 040 — `infer` (intermediate).
// Goal:   pull a type back out of a larger one instead of being told it.
// Drills: `infer` inside a conditional, one level at a time, the fallback
//         arm when nothing matches.
// Passes: all four extractors answer for the shapes they match and fall
//         back sensibly for the shapes they do not.
//
// `infer U` declares a type variable INSIDE the `extends` clause and binds
// it to whatever made the match succeed. It is pattern matching: you write
// the shape you expect with a hole in it, and the hole is what you get.
//
// It is only legal in the `extends` clause of a conditional type, and the
// binding is only in scope in the true arm. There is nothing like it in C#
// — the closest cousin is a `switch` pattern with a positional
// deconstruction, and even that matches values rather than types.
//
// This row is graded entirely at the type level. There is nothing to run:
// `infer` emits no code and exists only while the checker is working.
//
// Note that Resolved unwraps exactly ONE layer. Doing it repeatedly is
// ex070's recursive Awaited.

/** TODO: the element type of an array; never for anything else. */
export type ElementOf<T> = unknown;

/** TODO: what a Promise resolves to, one layer only; T itself otherwise. */
export type Resolved<T> = unknown;

/**
 * TODO: the type of a function's first parameter; never if it takes none or
 * if T is not a function.
 *
 * Measured, and it will catch you: matching
 * `(first: infer P, ...rest: never[]) => unknown` does NOT give never for a
 * zero-parameter function. A function that takes fewer parameters is
 * assignable to a signature that takes more, so `() => void` matches and P
 * infers as unknown. Infer the whole parameter tuple and take it apart.
 */
export type FirstParam<T> = unknown;

/** TODO: for a two-element tuple, its members swapped; never otherwise. */
export type Swapped<T> = unknown;

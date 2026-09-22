// Exercise 073 — recursion that keeps a tuple a tuple (advanced).
// Goal:   freeze a structure without flattening its fixed-shape parts.
// Drills: a homomorphic mapped type over an ARRAY type, arity
//         preservation, tuple labels.
// Passes: a tuple keeps its length and per-position types, an array stays
//         an array, and functions come through.
//
// ex065's DeepReadonly handled arrays with an explicit
// `T extends readonly (infer E)[] ? readonly DeepReadonly<E>[] : …`
// branch. That is correct for an array and LOSSY for a tuple:
// `[string, number]` comes out as `readonly (string | number)[]`, and the
// arity — the whole reason to use a tuple — is gone.
//
// The fix is smaller than the bug. A homomorphic mapped type is ARRAY
// AWARE: `{ readonly [K in keyof T]: F<T[K]> }` applied to an array type
// produces an array type, applied to a tuple produces a tuple OF THE SAME
// LENGTH with each position mapped, and applied to an object produces an
// object. One branch covers all three, and there is no need to test for
// an array at all.
//
// That is the same machinery as ex037's modifier preservation, and it is
// why `Readonly<[string, number]>` in the standard library does the right
// thing while a hand-rolled version usually does not.
//
// Functions still need their own guard, for ex065's reason.

/** TODO: readonly at every level, with tuples staying tuples. */
export type DeepFreeze<T> = unknown;

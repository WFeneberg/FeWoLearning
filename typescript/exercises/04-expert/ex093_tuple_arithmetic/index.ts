// Exercise 093 — counting with tuples (expert).
// Goal:   do arithmetic in a type system that has no arithmetic.
// Drills: building a tuple of a given length, `["length"]` as the way
//         back to a number, spreading to add and matching to subtract.
// Passes: addition, subtraction and comparison, within the range the
//         technique allows.
//
// TypeScript cannot add two numbers. It can, however, build a tuple of a
// given length and read that length back — and that is enough, because
// concatenating tuples adds their lengths.
//
//   Tuple<N>            a tuple of N elements
//   [...Tuple<A>, ...Tuple<B>]["length"]     A + B
//
// Subtraction runs the same machinery backwards: if `Tuple<A>` matches
// `[...Tuple<B>, ...infer Rest]`, then Rest has A - B elements. And
// because that match FAILS when B is larger, the natural answer for
// A - B below zero is `never` rather than a negative — which is honest,
// since the technique has no negatives in it.
//
// The costs are real and worth naming. Every operation is linear in the
// numbers involved, so this is unusable for large values; the depth
// limit (ex095) puts a hard ceiling on it; and it works on non-negative
// integer LITERALS only, since `number` has no length to build. Type
// arithmetic is for small, bounded things — array arities, recursion
// counters, protocol versions — and reaching for it elsewhere is a sign
// the value belongs at runtime.

/** TODO: a tuple with N elements. Build it by accumulating. */
export type Tuple<N extends number, T = unknown> = unknown;

/** TODO: A + B. */
export type Add<A extends number, B extends number> = unknown;

/** TODO: A - B, or never when B is the larger. */
export type Subtract<A extends number, B extends number> = unknown;

/** TODO: true when A is at least B. */
export type AtLeast<A extends number, B extends number> = unknown;

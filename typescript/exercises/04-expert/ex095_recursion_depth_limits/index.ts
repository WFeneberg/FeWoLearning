// Exercise 095 — where the compiler gives up (expert).
// Goal:   find the ceiling, and learn which shape raises it.
// Drills: the instantiation-depth error, the tail-recursive form that
//         survives it, and the honest limit of both.
// Passes: the naive counter works small and fails at the measured
//         depth, and the tail-recursive one goes far past it.
//
// MEASURED on this track's TypeScript 7.0.2, and these are the numbers
// the facts below use:
//
//   non-tail-recursive   works at 47, fails at 48
//   tail-recursive       works at 999, fails at 1000
//
// Roughly a factor of twenty-one, and both are hard walls rather than
// slowdowns: the error is
// `Type instantiation is excessively deep and possibly infinite`, and it
// appears at the USE, not at the declaration — so a type that is fine in
// its own file breaks in the one that instantiates it deeply enough.
// That is what makes this the hardest failure in the type system to
// diagnose from a bug report.
//
// Do not take those numbers as a specification. They are what this
// compiler does today; they have moved between versions and will move
// again. What is stable is the SHAPE: an accumulator-carrying recursion
// (ex094) buys roughly an order of magnitude, and nothing buys more.
//
// The conclusion worth carrying out of the expert tier: if a type needs
// more than a few hundred steps, the answer is not a cleverer type. It
// is that the work belongs at runtime, or that the input should be
// bounded before it gets here. A type that is one edit away from
// TS2589 is a liability whatever it computes.

// One mechanical trap, measured while building this: reading the result
// with a direct `Builder<N>["length"]` fails at the DECLARATION of the
// tail-recursive version — "Excessive stack depth comparing types" —
// because the unrolling loses track of the result being an array and
// `"length"` can no longer be proven to index it. Deferring the read
// into a conditional, `T extends { length: infer L } ? L : never`,
// sidesteps it. The nested version does not have the problem, which
// makes it that much more confusing when it appears.

/** TODO: count to N by nesting — the recursive call wrapped in a tuple,
 *  deliberately NOT tail-recursive. */
export type NaiveLength<N extends number> = unknown;

/** TODO: the same count, tail-recursively. */
export type TailLength<N extends number> = unknown;

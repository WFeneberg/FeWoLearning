// Exercise 094 — tail recursion at the type level (expert).
// Goal:   write the recursive types that survive depth, and see what
//         makes the difference.
// Drills: an accumulator parameter, the shape the compiler recognises,
//         a counter built from ex093's tuples.
// Passes: reversing, repeating and summing, all with accumulators.
//
// TypeScript optimises TAIL-RECURSIVE conditional types: when the true
// or false branch is a bare recursive call and nothing wraps it, the
// compiler unrolls it as a loop instead of a nested instantiation. The
// difference is not small — ex095 has the measured numbers — and the
// shape that gets it is specific.
//
//   Tail<T, Acc> = T extends [] ? Acc : Tail<Rest, [...Acc, Head]>
//                                       ^^^^ the whole branch
//
//   Not<T, Acc>  = T extends [] ? Acc : [...Not<Rest, Acc>, Head]
//                                       ^^^^ wrapped: not tail-recursive
//
// So the pattern is an ACCUMULATOR: carry the answer-so-far in an extra
// type parameter with a default, build it up on the way DOWN, and
// return it at the base case. Nothing is assembled on the way back up,
// because there is no way back up.
//
// This is the same transformation as making a runtime function tail
// recursive, and it has the same feel: the version with the accumulator
// is slightly less pleasant to read and is the one that works.
//
// Note that the extra parameters are implementation detail. Give them
// defaults so callers write `Reverse<[1, 2, 3]>` and never see them.

/** TODO: T with its elements in the opposite order. */
export type Reverse<T extends readonly unknown[]> = unknown;

/** TODO: S repeated N times. N counts down using a tuple (ex093). */
export type Repeat<S extends string, N extends number> = unknown;

/** TODO: the sum of a tuple of numbers, as a number literal. */
export type SumOf<T extends readonly number[]> = unknown;

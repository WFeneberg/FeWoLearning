// Reference solution — exercise 095.
// Reading the length through a conditional rather than with a direct
// `["length"]`. Measured: indexing the TAIL-recursive builder directly
// fails at its own declaration with "Excessive stack depth comparing
// types" — the unrolling loses track of the result being an array, and
// `"length"` can no longer be proven to index it. Deferring the read
// into a conditional sidesteps that entirely.
type LengthOf<T> = T extends { length: infer L } ? L : never;

// The recursive call is WRAPPED in a tuple, so nothing is unrolled.
// Note the base case: it returns the EMPTY tuple and each level
// prepends one element on the way back up. Returning Acc here instead
// would count everything twice.
type NaiveBuild<
  N extends number,
  Acc extends readonly unknown[] = [],
> = Acc["length"] extends N ? [] : [unknown, ...NaiveBuild<N, [unknown, ...Acc]>];

export type NaiveLength<N extends number> = LengthOf<NaiveBuild<N>>;

// The recursive call IS the branch, so the compiler unrolls it.
type TailBuild<
  N extends number,
  Acc extends readonly unknown[] = [],
> = Acc["length"] extends N ? Acc : TailBuild<N, [unknown, ...Acc]>;

export type TailLength<N extends number> = LengthOf<TailBuild<N>>;

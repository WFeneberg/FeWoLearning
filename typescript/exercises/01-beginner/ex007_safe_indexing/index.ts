// Exercise 007 — indexing is not a promise (beginner).
// Goal:   handle the undefined that an out-of-range read actually produces.
// Drills: noUncheckedIndexedAccess, widening a read instead of asserting it.
// Passes: at() reports T | undefined, and the two callers deal with the gap.
//
// This track sets noUncheckedIndexedAccess, so `items[i]` is T | undefined
// however confident the surrounding code looks. JavaScript has always behaved
// this way — `[1, 2][7]` is undefined, not an exception, which is the opposite
// of C#'s IndexOutOfRangeException. The flag only stops the type from lying
// about it.
//
// Note that at() below has NO return type annotation, on purpose: what it
// reports is inferred from your body, so writing `items[index]!` will be
// visible in the graded type.

/** The element at `index`, or undefined when there is none. */
export function at<T>(_items: readonly T[], _index: number) {
  throw new Error("TODO: implement at");
}

/** The first element, or `fallback` when the list is empty. */
export function firstOr(_items: readonly string[], _fallback: string): string {
  throw new Error("TODO: implement firstOr");
}

/** Sum of the values at `indices`. An index with no value contributes nothing. */
export function sumAt(_values: readonly number[], _indices: readonly number[]): number {
  throw new Error("TODO: implement sumAt");
}

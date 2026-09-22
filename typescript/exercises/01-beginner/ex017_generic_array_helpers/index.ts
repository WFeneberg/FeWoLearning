// Exercise 017 — generic helpers over arrays (beginner).
// Goal:   write list helpers that keep the element type their caller had.
// Drills: a type parameter inferred from an array, readonly parameters,
//         a return type that admits the undefined an empty list produces.
// Passes: first and last report T | undefined, and chunk reports T[][].
//
// Note the interaction with ex007: because this track sets
// noUncheckedIndexedAccess, `items[0]` is T | undefined, and first() must
// say so rather than assert it away. Accepting `readonly T[]` rather than
// `T[]` costs nothing and lets callers pass a frozen or `as const` array.
//
// As in ex016 the stubs are declared with `unknown`, which accepts every call
// and discards what the caller knew.

/** TODO: the first element, or undefined when there is none. */
export function first(_items: readonly unknown[]): unknown {
  throw new Error("TODO: implement first");
}

/** TODO: the last element, or undefined when there is none. */
export function last(_items: readonly unknown[]): unknown {
  throw new Error("TODO: implement last");
}

/**
 * TODO: split into runs of at most `size`, keeping order — chunk([1,2,3], 2)
 * is [[1, 2], [3]]. An empty input gives an empty result.
 */
export function chunk(_items: readonly unknown[], _size: number): unknown[][] {
  throw new Error("TODO: implement chunk");
}

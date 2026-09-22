// Exercise 006 — tuples (beginner).
// Goal:   describe a fixed-shape list, and a list guaranteed not to be empty.
// Drills: fixed-length tuples, labelled elements, a rest element in a tuple.
// Note:   element labels are documentation only. Measured: [number, number]
//         satisfies the Coordinate fact exactly as [latitude: number,
//         longitude: number] does, because labels are erased for
//         assignability. Write them for the reader, not for the grader.
// Passes: Coordinate is a two-number tuple, NonEmptyStrings needs a head, and
//         head() can return a string rather than string | undefined.
//
// A tuple is an array type whose length and per-position types are known. That
// knowledge is what lets head() below promise a string: with a plain string[]
// this track's noUncheckedIndexedAccess would make items[0] string | undefined
// (see ex007), but a tuple with a required first element cannot be empty.

/** TODO: a pair of numbers, latitude first, longitude second. */
export type Coordinate = unknown;

/** TODO: a list of strings with at least one element. */
export type NonEmptyStrings = unknown;

/** Returns `lat,lon`, each rounded to two decimals — e.g. `47.38,8.54`. */
export function formatCoordinate(_at: Coordinate): string {
  throw new Error("TODO: implement formatCoordinate");
}

/** The first element. Never undefined, because the type forbids an empty list. */
export function head(_items: NonEmptyStrings): string {
  throw new Error("TODO: implement head");
}

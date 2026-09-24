// Exercise 041 — async iteration (intermediate).
// Goal:   stream values that each need awaiting.
// Drills: async function*, for await..of, Symbol.asyncIterator, and the
//         cleanup an early break triggers.
// Passes: asyncTake() stops pulling AND lets the source run its finally
//         block — a for await..of that breaks calls the source's return().

/**
 * Yields each promise's value, in order, awaiting them one at a time.
 *
 * TODO: implement as an async generator.
 */
export async function* fromPromises(_promises) {
  throw new Error("TODO: implement fromPromises");
}

/**
 * Drains an async iterable into an array.
 *
 * TODO: implement with for await..of.
 */
export async function collect(_asyncIterable) {
  throw new Error("TODO: implement collect");
}

/**
 * Yields at most the first `count` values of an async iterable, then stops
 * pulling — including from an endless source.
 *
 * TODO: implement as an async generator whose loop breaks.
 */
export async function* asyncTake(_asyncIterable, _count) {
  throw new Error("TODO: implement asyncTake");
}

/**
 * An OBJECT (not a generator) that is async-iterable and yields
 * 0 … limit-1, awaiting a microtask between values.
 *
 * TODO: implement with [Symbol.asyncIterator]() returning an object whose
 * next() is async.
 */
export function makeAsyncRange(_limit) {
  throw new Error("TODO: implement makeAsyncRange");
}

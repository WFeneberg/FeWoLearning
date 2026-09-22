// Exercise 064 — the seed decides everything (intermediate).
// Goal:   write a fold whose accumulator type comes from the caller.
// Drills: two independent type parameters, inference from a seed,
//         grouping into a Record.
// Passes: fold reports the accumulator's own type, and groupBy builds a
//         record keyed by whatever the selector returns.
//
// In `fold(items, seed, step)` the element type T comes from `items` and
// the accumulator type A comes from the SEED — there is nowhere else for
// it to come from, since `step` is contextually typed from both (ex051).
// Get the seed wrong and everything downstream follows it silently.
//
// The classic bite, and it is worth trying in a scratch file:
//
//   [1, 2, 3].reduce((acc, n) => [...acc, n], [])
//
// The empty seed infers `never[]`, so `[...acc, n]` is an error on the
// FIRST line of the callback, complaining about a type nobody wrote.
// `[] as number[]` fixes it, and so does an explicit type argument. The
// error message never mentions the seed, which is why this costs people
// an afternoon.
//
// As elsewhere, the stubs take and return `unknown`.

/** TODO: fold `items` into an accumulator, starting from `seed`. */
export function fold(_items: unknown, _seed: unknown, _step: unknown): unknown {
  throw new Error("TODO: implement fold");
}

/** TODO: group items into a record keyed by `selectKey`, each entry an
 *  array in input order. */
export function groupBy(_items: unknown, _selectKey: unknown): unknown {
  throw new Error("TODO: implement groupBy");
}

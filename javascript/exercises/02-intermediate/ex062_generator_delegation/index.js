// Exercise 062 — yield* and two-way generators (intermediate).
// Goal:   compose generators, and send values back INTO one.
// Drills: yield* over a generator and over any iterable, the value a
//         yield* expression evaluates to, gen.next(value).
// Passes: countThenTotal() reads the delegated generator's RETURN value,
//         which is not one of the yielded values.

/**
 * Yields every value of a nested structure of arrays, depth-first.
 * flatten([1, [2, [3]], 4]) -> 1, 2, 3, 4
 *
 * TODO: implement recursively with yield*.
 */
export function* flatten(_value) {
  throw new Error("TODO: implement flatten");
}

/**
 * Yields each of `values` and RETURNS their sum — so a delegating generator
 * can read the total.
 *
 * TODO: implement.
 */
export function* yieldAndSum(_values) {
  throw new Error("TODO: implement yieldAndSum");
}

/**
 * Delegates to yieldAndSum(values) and then yields the string
 * `total: <sum>`.
 *
 * TODO: implement — `const sum = yield* yieldAndSum(values)`.
 */
export function* countThenTotal(_values) {
  throw new Error("TODO: implement countThenTotal");
}

/**
 * A generator that yields nothing useful on the way in and returns the list
 * of everything it was SENT: the consumer calls next(value) repeatedly, and
 * a final next("done") ends it.
 *
 * Yield "ready" first, then each subsequent value it receives back as
 * `echo: <value>`, until it is sent "done" — then return the array of every
 * received value, "done" excluded.
 *
 * TODO: implement.
 */
export function* echo() {
  throw new Error("TODO: implement echo");
}

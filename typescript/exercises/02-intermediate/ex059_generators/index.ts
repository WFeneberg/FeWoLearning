// Exercise 059 — generators and their three type parameters (intermediate).
// Goal:   produce values lazily, and take values back from the consumer.
// Drills: `function*`, `yield` as an expression, Generator<Y, R, N>.
// Passes: countTo yields then returns, and runningTotal accumulates the
//         values its consumer hands back.
//
// `Generator<Y, R, N>` has three parameters and most code only ever uses
// the first:
//
//   Y — what `yield` produces, and what shows up as `.value` while
//       `done` is false
//   R — what the generator RETURNS, which is `.value` on the step where
//       `done` becomes true. It is not part of a `for…of` iteration at
//       all: that loop discards it.
//   N — what the consumer may pass INTO `next(…)`, which is the type of
//       the `yield` expression itself.
//
// N is the one worth having. `const received = yield current;` makes a
// generator two-way: it hands out a value and waits for one back. The
// first `next()` has nothing to send — its argument is discarded, because
// there is no suspended `yield` waiting for it — so a two-way generator
// always yields once before the conversation starts.
//
// There is no C# equivalent of N: an iterator block only produces.

/** TODO: yield 1, 2, … up to and including n, then RETURN their total.
 *  The consumer sends nothing, so N is void. */
export function* countTo(_n: number): Generator<unknown, unknown, unknown> {
  throw new Error("TODO: implement countTo");
}

/**
 * TODO: yield the running total, starting with `start`. Each value the
 * consumer sends with next(value) is added, and the new total yielded.
 * Runs forever; the consumer stops it.
 */
export function* runningTotal(_start: number): Generator<unknown, unknown, unknown> {
  throw new Error("TODO: implement runningTotal");
}

// Exercise 034 — the iteration protocol (beginner).
// Goal:   make your own type work with for…of and the spread operator.
// Drills: Symbol.iterator, Iterable<T>, consuming an iterable generically.
// Passes: Range iterates its half-open interval, and the two helpers accept
//         anything iterable, not just arrays.
//
// `for…of`, spread, destructuring and Array.from all speak one protocol: a
// method under the well-known symbol Symbol.iterator returning an object
// with next(). Implement it and your type joins in everywhere, with nothing
// to register and no interface to inherit — the same structural deal as
// ex001, applied to a symbol-keyed method.
//
// The cheapest correct implementation is a generator (`*[Symbol.iterator]()`),
// because a generator object is already a valid iterator AND iterable.
//
// Note that Range's stub already declares the method, so that the spread in
// the tests typechecks; only the body is missing. The two helpers take
// `Iterable`, not arrays — accept the protocol, not one implementation.

export class Range {
  constructor(
    public readonly from: number,
    public readonly to: number,
  ) {}

  /** TODO: yield from, from+1, … up to but NOT including `to`. */
  *[Symbol.iterator](): Iterator<number> {
    throw new Error("TODO: implement Range[Symbol.iterator]");
  }
}

/** TODO: the total of everything the iterable yields. */
export function sumOf(_values: Iterable<number>): number {
  throw new Error("TODO: implement sumOf");
}

/**
 * TODO: the first `count` items, keeping the element type.
 *
 * STOP EARLY. `[...values].slice(0, count)` is the obvious shape and it is a
 * trap: one of the facts below passes an endless generator, and spreading
 * that does not fail the test — measured, it takes the whole test PROCESS
 * down with a V8 out-of-memory crash (exit 134), with no catchable error and
 * no timeout. If a run ends in a heap dump rather than a red fact, this is
 * why. Break out of the loop instead.
 */
export function take(_values: Iterable<unknown>, _count: number): unknown[] {
  throw new Error("TODO: implement take");
}

// Exercise 061 — the protocol underneath the generator (intermediate).
// Goal:   write an iterator by hand and see what `for…of` really does with
//         it.
// Drills: Symbol.iterator returning a fresh iterator, the iterable/iterator
//         distinction, and the `return()` call a break makes.
// Passes: ManualRange can be walked twice, and a closable iterator learns
//         about an early exit but not about a full drain.
//
// ex034 made a class iterable with one generator method. This row does the
// same job by hand, because the shape is worth seeing once: an ITERABLE
// has a `[Symbol.iterator]()` that hands back an ITERATOR, and an iterator
// has `next()` returning `{ value, done }`.
//
// The distinction is not academic. An iterable that returns a FRESH
// iterator each time can be walked repeatedly; one that returns itself —
// which every generator object does — is single-use, and the second
// `for…of` over it sees nothing. That is why `[...gen]` twice gives you a
// list and then an empty list.
//
// The part almost nobody knows: leaving a `for…of` early — `break`,
// `return`, or a throw — calls the iterator's optional `return()` method.
// That is the protocol's cleanup hook, and it is how a generator's
// `finally` block runs when a consumer walks away. A full drain does NOT
// call it, because the iterator already said it was done.

export class ManualRange implements Iterable<number> {
  constructor(
    public readonly from: number,
    public readonly to: number,
  ) {}

  /** TODO: hand back a NEW iterator over from…to-1 each time it is asked.
   *  Write the `next()` yourself; do not use a generator. */
  [Symbol.iterator](): Iterator<number> {
    throw new Error("TODO: implement ManualRange[Symbol.iterator]");
  }
}

/**
 * TODO: an iterator over `values` that is also iterable (it returns
 * itself, so it is single-use), and that calls `onClose` exactly once if a
 * consumer leaves early. A consumer that drains it fully must NOT trigger
 * onClose.
 */
export function closable(
  _values: readonly number[],
  _onClose: () => void,
): IterableIterator<number> {
  throw new Error("TODO: implement closable");
}

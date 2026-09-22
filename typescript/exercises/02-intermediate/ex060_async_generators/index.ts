// Exercise 060 — async iteration (intermediate).
// Goal:   produce a stream whose next value is not ready yet.
// Drills: `async function*`, `for await…of`, composing async generators.
// Passes: inOrder yields in input order however the promises settle,
//         collect drains any async iterable, and filterAsync composes.
//
// An async generator is the two halves of this tier joined: `yield` from
// ex059 and `await` from ex031, in one function. It implements
// Symbol.asyncIterator rather than ex034's Symbol.iterator, and
// `for await…of` is what consumes it.
//
// The important property is BACKPRESSURE, and it is free: the generator
// does not run ahead. It produces one value, suspends, and resumes only
// when the consumer asks for the next one — so a slow consumer cannot be
// buried by a fast producer. That is the difference from handing someone
// an array of promises, which all start at once.
//
// inOrder below makes the ordering visible: it awaits the promises in
// ARGUMENT order, so a later promise settling first still waits its turn.
// That is a real trade — it is the right answer for a stream that must
// stay ordered, and the wrong one when you want whatever arrives next.
//
// The helpers take `AsyncIterable`, not the generator type, so anything
// implementing the protocol works — the same structural deal as ex034.
//
// One thing the facts below do NOT grade, said plainly: laziness. The
// promises handed to inOrder have already started, so awaiting them in a
// loop and awaiting them all at once produce the same order and the same
// values. Backpressure is real and it is why you would reach for this
// shape, but nothing here can see it.

/** TODO: yield each promise's value, in argument order. */
export async function* inOrder(
  _promises: readonly Promise<unknown>[],
): AsyncGenerator<unknown, unknown, unknown> {
  throw new Error("TODO: implement inOrder");
}

/** TODO: everything the source yields, as an array. */
export async function collect(_source: AsyncIterable<unknown>): Promise<unknown[]> {
  throw new Error("TODO: implement collect");
}

/** TODO: only the items the predicate accepts, keeping order. */
export async function* filterAsync(
  _source: AsyncIterable<unknown>,
  _keep: (item: never) => boolean,
): AsyncGenerator<unknown, unknown, unknown> {
  throw new Error("TODO: implement filterAsync");
}

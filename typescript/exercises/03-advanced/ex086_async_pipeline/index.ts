// Exercise 086 — composing async generators (advanced).
// Goal:   build stream operators that chain, and make an early exit
//         reach all the way upstream.
// Drills: async generators as operators, `for await` over another
//         operator, `finally` as the cleanup hook.
// Passes: the operators compose, and stopping the consumer runs the
//         upstream generator's finally block.
//
// ex060 wrote three async helpers. This row is about them COMPOSING:
// each operator takes an AsyncIterable and returns one, so a pipeline is
// ordinary function application and every stage stays lazy.
//
// The part worth building deliberately is TERMINATION. When a consumer
// stops early — a `break`, or an operator like takeAsync that simply
// stops asking — the generator it was reading is closed: its `return()`
// is called (ex061), which resumes it at the `yield` as though a return
// statement had run there, so a `finally` around the loop EXECUTES. And
// because each operator is itself iterating the stage above it, that
// closure propagates up the whole chain, one generator at a time.
//
// That is the mechanism behind "a stream cleans itself up", and it is
// why cleanup belongs in a `finally` rather than after the loop: after
// the loop never runs for a consumer that walked away.
//
// A warning about the shape of your loops. An operator that gathers its
// input before yielding anything — draining the source into an array
// first — still produces the right values and destroys both properties.
// It is no longer lazy, and an endless source hangs it forever rather
// than failing. One fact below feeds exactly that.

/** TODO: apply `transform` to every item, lazily. */
export async function* mapAsync(
  _source: AsyncIterable<unknown>,
  _transform: (item: never) => unknown,
): AsyncGenerator<unknown, unknown, unknown> {
  throw new Error("TODO: implement mapAsync");
}

/** TODO: yield at most `count` items, then stop asking. */
export async function* takeAsync(
  _source: AsyncIterable<unknown>,
  _count: number,
): AsyncGenerator<unknown, unknown, unknown> {
  throw new Error("TODO: implement takeAsync");
}

/** TODO: drain a source into an array. */
export async function collectAsync(_source: AsyncIterable<unknown>): Promise<unknown[]> {
  throw new Error("TODO: implement collectAsync");
}

/**
 * TODO: an endless source of 0, 1, 2, … that calls `onClose` from a
 * `finally` when whoever is reading it stops early.
 */
export async function* naturalsWithCleanup(
  _onClose: () => void,
): AsyncGenerator<number, void, void> {
  throw new Error("TODO: implement naturalsWithCleanup");
}

// Exercise 057 — when everything fails (intermediate).
// Goal:   handle the one rejection that carries a collection of other
//         rejections.
// Drills: AggregateError, its `errors` array, and the empty-input edge.
// Passes: the happy path returns a value, the all-failed path summarises
//         every reason, and an empty input is treated as a failure.
//
// `Promise.any` rejects only when every input does, and it rejects with an
// AggregateError — a standard Error subclass carrying an `errors` array of
// the individual reasons, in input order. It is the one place in the
// language where a single rejection genuinely holds several.
//
// Two things to watch. `errors` is typed `any[]` in the standard library,
// because a rejection can be anything (ex029), so reading a message off
// one means narrowing it first. And `Promise.any([])` rejects
// IMMEDIATELY with an AggregateError whose `errors` is empty — an empty
// input is a failure, not a success, which is the opposite of
// `Promise.all([])`.
//
// Neither helper below carries a return annotation: the graded type is the
// one your body produces.

/** TODO: the messages of every error in an AggregateError, in order.
 *  A non-Error entry contributes String(entry). Not an AggregateError at
 *  all: an empty list. */
export function reasonsOf(_error: unknown) {
  throw new Error("TODO: implement reasonsOf");
}

/**
 * TODO: the first promise to succeed. If every one fails, return
 * `none:<reason>|<reason>|…` built from the AggregateError's messages in
 * order. For no promises at all, return `none:`.
 */
export async function firstSuccessOrSummary(_promises: readonly Promise<string>[]) {
  throw new Error("TODO: implement firstSuccessOrSummary");
}

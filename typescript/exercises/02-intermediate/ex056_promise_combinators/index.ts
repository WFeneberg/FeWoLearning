// Exercise 056 — the four combinators, and which one you actually want
// (intermediate).
// Goal:   pick between all, allSettled, race and any on purpose.
// Drills: allSettled's per-item outcome, race settling on a rejection, any
//         skipping one.
// Passes: partition reports both halves, firstSettled loses to a fast
//         failure, and firstSuccess does not.
//
// Four combinators, three genuinely different promises:
//
//   all        — every value, or the first rejection. Fails fast (ex033).
//   allSettled — one OUTCOME per input. Never rejects.
//   race       — the first to SETTLE, success or failure alike.
//   any        — the first to SUCCEED, skipping rejections; rejects only
//                when every single one fails (ex057).
//
// The pair that catches people is race and any. `race` is not "the first
// good answer" — a fast failure wins it, which is exactly what you want
// for a timeout and exactly what you do not want for a set of mirrors.
//
// partition below cannot be built on `all`: it has to report both halves,
// and `all` throws away everything the moment one input rejects. That is
// what makes its facts grade the choice of combinator rather than the
// shape of the output.
//
// Note that a PromiseSettledResult is a discriminated union (ex015) tagged
// `status`, so reading `.value` means narrowing first.

/** TODO: the values that resolved and the messages of those that did not,
 *  each in input order. An Error rejection contributes its message; any
 *  other rejection contributes String(reason). */
export async function partition(_promises: readonly Promise<number>[]) {
  throw new Error("TODO: implement partition");
}

/** TODO: the first promise to settle — `ok:<value>` if it resolved,
 *  `fail:<message>` if it rejected. */
export async function firstSettled(_promises: readonly Promise<string>[]): Promise<string> {
  throw new Error("TODO: implement firstSettled");
}

/** TODO: the first promise to RESOLVE, ignoring any that reject first. */
export async function firstSuccess(_promises: readonly Promise<string>[]): Promise<string> {
  throw new Error("TODO: implement firstSuccess");
}

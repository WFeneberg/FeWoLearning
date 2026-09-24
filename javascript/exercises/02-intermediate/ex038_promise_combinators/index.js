// Exercise 038 — the four combinators (intermediate).
// Goal:   pick the one whose failure behaviour you actually want.
// Drills: all (fails fast), allSettled (never fails), race (first to
//         SETTLE), any (first to FULFIL, AggregateError when none does).
// Passes: collectAll() rejects on the first failure while
//         collectSettled() reports it as one entry among many.

/**
 * Resolves to an array of every value, in input order. Rejects with the
 * first rejection as soon as it happens.
 *
 * TODO: implement with Promise.all.
 */
export function collectAll(_promises) {
  throw new Error("TODO: implement collectAll");
}

/**
 * Never rejects. Resolves to
 *   { fulfilled: [values…], rejected: [reasons…] }
 * once every promise has settled, keeping input order within each list.
 *
 * TODO: implement with Promise.allSettled.
 */
export function collectSettled(_promises) {
  throw new Error("TODO: implement collectSettled");
}

/**
 * Settles the way the FIRST promise to settle did — a rejection wins the
 * race just as well as a value.
 *
 * TODO: implement with Promise.race.
 */
export function firstSettled(_promises) {
  throw new Error("TODO: implement firstSettled");
}

/**
 * Resolves with the first promise to FULFIL, ignoring rejections. If every
 * one rejects, rejects with an AggregateError whose `errors` holds them all.
 *
 * TODO: implement with Promise.any.
 */
export function firstSuccess(_promises) {
  throw new Error("TODO: implement firstSuccess");
}

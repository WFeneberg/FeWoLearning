// Exercise 077 — a concurrency pool (advanced).
// Goal:   run N tasks with at most K in flight.
// Drills: starting only as many as allowed, refilling as each settles,
//         keeping results in input order, and not deadlocking on a failure.
// Passes: the tests count how many tasks are running at once, so neither a
//         fully sequential nor a fully parallel implementation passes.

/**
 * Runs `worker(item, index)` over every item with at most `limit` calls in
 * flight at a time, and resolves to the results in INPUT order.
 *
 * A worker rejection rejects the whole thing (like Promise.all), and the
 * pool must not hang afterwards.
 *
 * TODO: implement — a shared cursor and `limit` runners that each pull the
 * next index is the simplest shape.
 */
export function mapWithConcurrency(_items, _limit, _worker) {
  throw new Error("TODO: implement mapWithConcurrency");
}

/**
 * The same, but nothing rejects: each entry of the result is
 * { status: "fulfilled", value } or { status: "rejected", reason }.
 *
 * TODO: implement.
 */
export function settleWithConcurrency(_items, _limit, _worker) {
  throw new Error("TODO: implement settleWithConcurrency");
}

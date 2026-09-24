// Exercise 080 — a push stream with teardown (advanced).
// Goal:   the shape behind every observable library, in thirty lines.
// Drills: subscribe returning an unsubscribe, a producer's teardown, the
//         contract that nothing arrives after complete or error, and
//         multicast vs the cold source.
// Passes: unsubscribing runs the producer's teardown exactly once, and a
//         late next() after complete() is dropped rather than delivered.

/**
 * Creates a COLD stream: `producer` is called per subscription with
 * { next, complete, error } and may return a teardown function.
 *
 * Returns { subscribe } where subscribe(observer) returns an unsubscribe
 * function. `observer` is { next?, complete?, error? } — any of them may
 * be missing.
 *
 * Rules:
 *   - after complete() or error(), no further callback is delivered
 *   - complete() and error() run the teardown
 *   - unsubscribe() runs the teardown and stops delivery
 *   - the teardown runs at most once per subscription
 *
 * TODO: implement.
 */
export function createStream(_producer) {
  throw new Error("TODO: implement createStream");
}

/**
 * A HOT source everyone shares: { subscribe, next, complete }.
 * Every subscriber sees the same values from the moment it subscribed;
 * nothing is replayed.
 *
 * TODO: implement.
 */
export function createSubject() {
  throw new Error("TODO: implement createSubject");
}

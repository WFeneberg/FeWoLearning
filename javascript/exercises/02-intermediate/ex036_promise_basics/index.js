// Exercise 036 — promises, by hand (intermediate).
// Goal:   build and chain a promise without async/await in the way.
// Drills: the Promise constructor, resolve/reject, then/catch/finally,
//         settle-once, a deferred.
// Passes: settleOnce() proves the second resolve and the later reject are
//         ignored, and runFinally() does not change the value it passes on.

/**
 * Returns { promise, resolve, reject } where the two functions settle the
 * promise from outside. This is the "deferred" shape every test in this
 * track uses instead of a timer.
 *
 * TODO: implement with the Promise constructor. (The platform now has
 * Promise.withResolvers() for this — build it by hand once first.)
 */
export function deferred() {
  throw new Error("TODO: implement deferred");
}

/**
 * Returns a promise whose executor calls resolve("first"), then
 * resolve("second"), then reject(new Error("too late")).
 *
 * A promise settles once; everything after the first call is discarded,
 * and the late rejection does NOT become an unhandled rejection.
 *
 * TODO: implement.
 */
export function settleOnce() {
  throw new Error("TODO: implement settleOnce");
}

/**
 * A new promise for fn(value) once `promise` fulfils. A `fn` that throws
 * must produce a rejected promise, not a synchronous throw.
 *
 * TODO: implement with .then.
 */
export function mapResolved(_promise, _fn) {
  throw new Error("TODO: implement mapResolved");
}

/**
 * Resolves to `fallback` if `promise` rejects, otherwise to its value.
 *
 * TODO: implement with .catch.
 */
export function withFallback(_promise, _fallback) {
  throw new Error("TODO: implement withFallback");
}

/**
 * Calls `onDone()` when `promise` settles either way, and passes the
 * original outcome through unchanged — a fulfilled value stays that value,
 * a rejection stays that rejection.
 *
 * TODO: implement with .finally.
 */
export function runFinally(_promise, _onDone) {
  throw new Error("TODO: implement runFinally");
}

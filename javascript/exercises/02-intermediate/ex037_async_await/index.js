// Exercise 037 — async / await (intermediate).
// Goal:   the sugar, and the two places it surprises people.
// Drills: async functions always return a promise, a throw inside one
//         becomes a rejection, await accepts non-promises, awaiting in a
//         loop, the [error, value] tuple pattern.
// Passes: failsLater() does NOT throw synchronously, and awaitPlain()
//         shows that await works on any value at all.

/**
 * Awaits `api.getUser(id)` then `api.getOrders(user.id)` and returns
 * { user, orders }. The second call needs the first one's result.
 *
 * TODO: implement.
 */
export async function loadProfile(_api, _id) {
  throw new Error("TODO: implement loadProfile");
}

/**
 * Awaits `promise` and returns [null, value], or [error, undefined] when it
 * rejects. Never rejects itself.
 *
 * TODO: implement.
 */
export async function safeAwait(_promise) {
  throw new Error("TODO: implement safeAwait");
}

/**
 * An async function that THROWS `new RangeError(message)`.
 *
 * Calling it must not throw synchronously — it returns a rejected promise,
 * which is the whole difference between a normal and an async function.
 *
 * TODO: implement.
 */
export async function failsLater(_message) {
  throw new Error("TODO: implement failsLater");
}

/**
 * Awaits `value` — which need not be a promise at all — and returns it.
 * Also pushes "before" onto `log` before the await and "after" onto it
 * afterwards, so the tests can see that the await really did suspend.
 *
 * TODO: implement.
 */
export async function awaitPlain(_value, _log) {
  throw new Error("TODO: implement awaitPlain");
}

/**
 * Sums `api.getValue(id)` over every id, IN SEQUENCE — each call starts
 * only after the previous one has settled.
 *
 * TODO: implement with for..of and await.
 */
export async function sumSequential(_api, _ids) {
  throw new Error("TODO: implement sumSequential");
}

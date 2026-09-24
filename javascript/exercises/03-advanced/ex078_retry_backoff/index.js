// Exercise 078 — retry with backoff (advanced).
// Goal:   retry without a wall clock anywhere in the test.
// Drills: an injected sleep, an attempt budget, exponential delays, a
//         predicate deciding what is worth retrying, and the last error
//         surviving.
// Passes: the delays are recorded by the injected sleep, so the schedule
//         is graded rather than timed.

/**
 * Calls `fn(attempt)` — attempt starting at 1 — until it resolves or the
 * budget runs out. Options:
 *   attempts   how many calls in total (default 3)
 *   baseDelay  the first delay in ms (default 10)
 *   factor     the multiplier between delays (default 2)
 *   sleep      async (ms) => void, called BETWEEN attempts (required)
 *   shouldRetry (error, attempt) => boolean, default () => true
 *
 * The delays are baseDelay, baseDelay*factor, baseDelay*factor², … and
 * there is no sleep after the last attempt.
 *
 * On exhaustion — or when shouldRetry says no — it rejects with the LAST
 * error.
 *
 * TODO: implement.
 */
export function retry(_fn, _options) {
  throw new Error("TODO: implement retry");
}

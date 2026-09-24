// Exercise 055 — debounce and throttle (intermediate).
// Goal:   the two rate limiters, and the difference nobody remembers.
// Drills: setTimeout/clearTimeout in a closure, trailing vs leading edge,
//         cancelling, keeping the last arguments.
// Passes: the tests run on fake timers, so the behaviour is graded rather
//         than the wall clock.

/**
 * Trailing debounce: calls `fn` `ms` after the LAST call. Every new call
 * within the window restarts the clock, and `fn` receives the arguments of
 * the most recent call. The returned function also has `cancel()`.
 *
 * TODO: implement.
 */
export function debounce(_fn, _ms) {
  throw new Error("TODO: implement debounce");
}

/**
 * Leading throttle: calls `fn` immediately, then ignores calls for `ms`.
 * The first call after the window passes goes through again.
 *
 * TODO: implement.
 */
export function throttle(_fn, _ms) {
  throw new Error("TODO: implement throttle");
}

// Exercise 018 — `this` (beginner).
// Goal:   `this` is decided by the CALL, not by the definition.
// Drills: call/apply/bind, a detached method, borrowing an array method,
//         a bound function that cannot be rebound.
// Passes: invokeAs() can point any function at any receiver, and
//         bindMethod() keeps working after the method is torn off.

/**
 * Calls `fn` with `receiver` as its `this` and `args` as its arguments.
 *
 * TODO: implement with apply (or call plus a spread).
 */
export function invokeAs(_fn, _receiver, _args) {
  throw new Error("TODO: implement invokeAs");
}

/**
 * Returns `object[name]` permanently bound to `object`, so it keeps working
 * when it is passed around on its own.
 *
 * TODO: implement.
 */
export function bindMethod(_object, _name) {
  throw new Error("TODO: implement bindMethod");
}

/**
 * Returns the `this` a plain function sees when it is called with no
 * receiver at all. In a module — which is always strict mode — that is
 * undefined, not the global object.
 *
 * TODO: implement: define a normal function that returns its own `this`,
 * call it bare, and return the result.
 */
export function thisWhenCalledBare() {
  throw new Error("TODO: implement thisWhenCalledBare");
}

/**
 * Turns an array-LIKE object ({ 0: "a", 1: "b", length: 2 }) into a real
 * array by borrowing Array.prototype.slice with call().
 *
 * TODO: implement with the borrow, not with Array.from — the point is that
 * `this` is just an argument.
 */
export function borrowSlice(_arrayLike) {
  throw new Error("TODO: implement borrowSlice");
}

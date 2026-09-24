// Exercise 014 — default parameters (beginner).
// Goal:   know exactly when a default expression runs.
// Drills: defaults evaluated per call, earlier parameters in scope,
//         undefined triggers a default and null does not, Function.length.
// Passes: appendTo() gets a FRESH array on every call, and span() reads the
//         parameter to its left.
//
// Coming from Python: `def f(x, list=[])` shares one list across every call.
// JavaScript re-evaluates the expression each time, so the same-looking code
// does the thing people expect. This exercise proves it.

/**
 * Appends `value` to `list` and returns it. With no list, each call starts
 * a new empty one.
 *
 * TODO: implement with a default parameter.
 */
export function appendTo(_value, _list) {
  throw new Error("TODO: implement appendTo");
}

/**
 * span(5) -> { start: 5, end: 15 }
 * span(5, 7) -> { start: 5, end: 7 }
 *
 * TODO: implement — the default for `end` is computed from `start`.
 */
export function span(_start, _end) {
  throw new Error("TODO: implement span");
}

/**
 * greet() -> "Hello, guest!"
 * greet(null) -> "Hello, null!"   (null is a value; only undefined defaults)
 *
 * TODO: implement.
 */
export function greet(_name) {
  throw new Error("TODO: implement greet");
}

/**
 * Calls `make()` to produce the id only when no id was supplied — a default
 * expression is not evaluated at all when the argument is present.
 *
 * TODO: implement so that `make` is never called for a supplied id.
 */
export function withId(_make, _id) {
  throw new Error("TODO: implement withId");
}

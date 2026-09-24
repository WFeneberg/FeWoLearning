// Exercise 052 — currying and partial application (intermediate).
// Goal:   turn a three-argument function into three one-argument calls.
// Drills: nested closures, Function.length as the arity source, bind for
//         partial application, why an arity-driven curry stops when it does.
// Passes: curry() accepts the arguments in any grouping, and partial()
//         produces a function whose remaining arity is reported correctly.

/**
 * curry3(fn)(a)(b)(c) === fn(a, b, c). Exactly one argument per call.
 *
 * TODO: implement with nested arrows.
 */
export function curry3(_fn) {
  throw new Error("TODO: implement curry3");
}

/**
 * A general curry: calls `fn` once `fn.length` arguments have arrived, in
 * however many calls that takes. curry(f)(1)(2, 3) and curry(f)(1, 2)(3)
 * both work.
 *
 * TODO: implement — collect arguments and compare with fn.length.
 */
export function curry(_fn) {
  throw new Error("TODO: implement curry");
}

/**
 * Presets the first arguments of `fn`. The returned function's `length`
 * must report how many are still missing — which is what `bind` already
 * does for you.
 *
 * TODO: implement with bind.
 */
export function partial(_fn, ..._preset) {
  throw new Error("TODO: implement partial");
}

/**
 * Presets the LAST argument instead: partialRight(f, c)(a, b) === f(a, b, c).
 *
 * TODO: implement — bind cannot do this one.
 */
export function partialRight(_fn, ..._preset) {
  throw new Error("TODO: implement partialRight");
}

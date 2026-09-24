// Exercise 053 — composition (intermediate).
// Goal:   build a pipeline out of small functions.
// Drills: reduce for left-to-right, reduceRight for right-to-left, the
//         identity case, and awaiting each stage.
// Passes: pipe and compose apply the same functions in opposite orders, and
//         pipeAsync awaits every stage rather than passing promises on.

/**
 * pipe(f, g)(x) === g(f(x)) — left to right.
 * pipe()(x) === x.
 *
 * TODO: implement with reduce.
 */
export function pipe(..._fns) {
  throw new Error("TODO: implement pipe");
}

/**
 * compose(f, g)(x) === f(g(x)) — right to left, the maths convention.
 *
 * TODO: implement with reduceRight.
 */
export function compose(..._fns) {
  throw new Error("TODO: implement compose");
}

/**
 * Like pipe, but every stage may return a promise: each stage receives the
 * AWAITED result of the one before. Returns a promise.
 *
 * TODO: implement.
 */
export function pipeAsync(..._fns) {
  throw new Error("TODO: implement pipeAsync");
}

/**
 * Passes the value through `fn` for its side effect and returns the value
 * unchanged, so it can sit in the middle of a pipe.
 *
 * TODO: implement.
 */
export function tap(_fn) {
  throw new Error("TODO: implement tap");
}

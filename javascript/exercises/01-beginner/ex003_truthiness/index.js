// Exercise 003 — truthiness (beginner).
// Goal:   stop losing 0 and "" to a default.
// Drills: the falsy values, `||` vs `??`, `??=`, in-place defaulting.
// Passes: orDefault() and withDefault() disagree on 0 and "", and
//         applyDefaults() fills only the missing keys of the object it was
//         given — without replacing it.

/** The eight falsy values, in any order. TODO: return them. */
export function falsyValues() {
  throw new Error("TODO: implement falsyValues");
}

/**
 * The old way: `value || fallback`. Returns the fallback for ANY falsy value.
 *
 * TODO: implement with `||`.
 */
export function orDefault(_value, _fallback) {
  throw new Error("TODO: implement orDefault");
}

/**
 * The 2020 way: `value ?? fallback`. Returns the fallback only for null and
 * undefined, so a legitimate 0 or "" survives.
 *
 * TODO: implement with `??`.
 */
export function withDefault(_value, _fallback) {
  throw new Error("TODO: implement withDefault");
}

/**
 * Fills in `retries`, `timeoutMs` and `label` on the config object it is
 * given — only where the value is null or undefined — and returns THAT SAME
 * object, mutated. Defaults: retries 3, timeoutMs 1000, label "job".
 *
 * TODO: implement with `??=`.
 */
export function applyDefaults(_config) {
  throw new Error("TODO: implement applyDefaults");
}

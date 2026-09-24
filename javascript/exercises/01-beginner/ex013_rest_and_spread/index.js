// Exercise 013 — rest and spread (beginner).
// Goal:   the same three dots, in both directions.
// Drills: rest parameters, spreading into a call, spreading into literals,
//         rest properties with a computed key, what "shallow" costs.
// Passes: withoutKey() removes a key by value, and cloneShallow() shares the
//         nested objects it copied — deliberately.

/** Sums any number of arguments. sum() is 0. TODO: implement with a rest parameter. */
export function sum(..._numbers) {
  throw new Error("TODO: implement sum");
}

/**
 * The largest value in the array, or -Infinity for an empty one — which is
 * what Math.max() answers over nothing, and the right neutral element.
 *
 * TODO: implement by spreading into Math.max.
 */
export function maxOf(_values) {
  throw new Error("TODO: implement maxOf");
}

/**
 * Merges override onto base into a NEW object; override wins. An override
 * value of undefined still wins (spread copies it).
 *
 * TODO: implement with object spread.
 */
export function mergeConfig(_base, _override) {
  throw new Error("TODO: implement mergeConfig");
}

/**
 * A NEW object without `key`. The key name is only known at runtime.
 *
 * TODO: implement with a rest property and a computed key —
 * `const { [key]: _removed, ...rest } = object`.
 */
export function withoutKey(_object, _key) {
  throw new Error("TODO: implement withoutKey");
}

/**
 * A shallow copy: a new top-level object, but every nested value is the
 * SAME reference the original holds.
 *
 * TODO: implement with spread.
 */
export function cloneShallow(_object) {
  throw new Error("TODO: implement cloneShallow");
}

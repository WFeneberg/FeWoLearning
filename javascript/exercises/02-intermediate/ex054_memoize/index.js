// Exercise 054 — memoization (intermediate).
// Goal:   cache results without caching bugs.
// Drills: a Map-backed cache, `has` rather than a truthiness check, a
//         custom key function, a WeakMap for object arguments.
// Passes: a memoized function that returns undefined is still called only
//         once, and memoizeBy() honours the key function rather than the
//         argument.

/**
 * Caches `fn` by its single argument (compared as a Map key). The returned
 * function also has a `clear()` that empties the cache.
 *
 * TODO: implement. Careful: a cached `undefined` must not look like a miss.
 */
export function memoize(_fn) {
  throw new Error("TODO: implement memoize");
}

/**
 * Caches `fn` by `keyFn(...args)`, so a multi-argument function can be
 * memoized too.
 *
 * TODO: implement.
 */
export function memoizeBy(_fn, _keyFn) {
  throw new Error("TODO: implement memoizeBy");
}

/**
 * Caches `fn` by an OBJECT argument, held weakly — so a cached entry cannot
 * keep the object alive on its own.
 *
 * TODO: implement with a WeakMap.
 */
export function memoizeWeak(_fn) {
  throw new Error("TODO: implement memoizeWeak");
}

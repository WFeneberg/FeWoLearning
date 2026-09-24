// Exercise 059 — flat and flatMap (intermediate).
// Goal:   flatten by one level, by many, or while mapping.
// Drills: flat(depth), flat(Infinity), flatMap, returning [] from flatMap
//         to drop an item, and the one level flatMap does NOT flatten.
// Passes: flattenDeep() handles unknown depth, and compact() drops items
//         through flatMap rather than a second filter pass.

/** One level only: [[1], [2, [3]]] -> [1, 2, [3]]. TODO: implement. */
export function flattenOnce(_list) {
  throw new Error("TODO: implement flattenOnce");
}

/** Every level: [1, [2, [3, [4]]]] -> [1, 2, 3, 4]. TODO: implement. */
export function flattenDeep(_list) {
  throw new Error("TODO: implement flattenDeep");
}

/**
 * Maps each item to zero or more results in one pass:
 * expand([1, 2]) with fn = n => [n, n * 10] -> [1, 10, 2, 20]
 *
 * TODO: implement with flatMap.
 */
export function expand(_list, _fn) {
  throw new Error("TODO: implement expand");
}

/**
 * Keeps the items `predicate` accepts, mapping each through `fn`, in ONE
 * pass — a rejected item contributes nothing.
 *
 * TODO: implement with flatMap, returning [] for a rejected item.
 */
export function filterMap(_list, _predicate, _fn) {
  throw new Error("TODO: implement filterMap");
}

// Exercise 057 — deep clone, by hand (intermediate).
// Goal:   do what structuredClone does, and see the decisions it makes.
// Drills: recursion, a seen-map for shared references and cycles, cloning
//         Date/Map/Set, and choosing what NOT to clone.
// Passes: a shared reference stays SHARED in the copy (one clone, two
//         places), rather than being copied twice.

/**
 * A deep copy of plain data:
 *   - arrays, plain objects, Date, Map, Set are cloned
 *   - primitives are returned as they are
 *   - functions are NOT cloned: the same reference is reused
 *   - a cycle is preserved and points at the copy
 *   - two properties holding the same object hold the same COPY afterwards
 *
 * TODO: implement with a WeakMap of original -> copy.
 */
export function deepClone(_value) {
  throw new Error("TODO: implement deepClone");
}

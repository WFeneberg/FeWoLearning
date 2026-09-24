// Exercise 048 — freeze, seal, preventExtensions (intermediate).
// Goal:   know which of the three you actually want, and how far it reaches.
// Drills: Object.freeze/seal/preventExtensions, isFrozen/isSealed/
//         isExtensible, shallowness, and deep-freezing a cycle.
// Passes: freezeDeep() survives a self-referencing object, and
//         shallowGap() shows what plain freeze() does NOT cover.

/**
 * Classifies an object as { frozen, sealed, extensible }.
 *
 * TODO: implement.
 */
export function classify(_object) {
  throw new Error("TODO: implement classify");
}

/**
 * Freezes `object` and everything reachable from it, and returns it.
 * Must not loop forever on a cycle.
 *
 * TODO: implement.
 */
export function freezeDeep(_object) {
  throw new Error("TODO: implement freezeDeep");
}

/**
 * Freezes `{ nested: { value: 1 } }` shallowly, then writes 2 into the
 * nested object, and returns { frozenOuter, nestedValue }.
 *
 * freeze() reaches one level, so the write goes through and nestedValue is 2.
 *
 * TODO: implement.
 */
export function shallowGap() {
  throw new Error("TODO: implement shallowGap");
}

/**
 * Tries three things on `object` and reports each as "ok" or the thrown
 * error's name:
 *   { write, addKey, deleteKey }
 * write sets an existing key `a`, addKey sets a brand-new key `fresh`,
 * deleteKey deletes `a`.
 *
 * TODO: implement — each attempt needs its own try/catch.
 */
export function probeMutations(_object) {
  throw new Error("TODO: implement probeMutations");
}

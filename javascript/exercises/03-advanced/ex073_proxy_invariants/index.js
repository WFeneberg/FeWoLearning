// Exercise 073 — the invariants a Proxy cannot break (advanced).
// Goal:   learn where the runtime stops believing your traps.
// Drills: non-configurable non-writable properties, ownKeys and
//         non-configurable keys, isExtensible, and the TypeErrors each
//         violation raises.
// Passes: every function here reports what the runtime did, and the
//         answers are TypeErrors rather than the lies the traps told.
//
// The rule: a proxy may invent anything about a FLEXIBLE property, and
// nothing about a locked one. Code that checked a descriptor once has to
// be able to rely on it.

/**
 * Builds a target with a non-writable, non-configurable property `fixed`
 * set to "real", wraps it in a proxy whose get trap returns "fake", reads
 * it, and returns either the value or the caught error's name.
 *
 * TODO: implement. The runtime refuses the lie.
 */
export function lieAboutValue() {
  throw new Error("TODO: implement lieAboutValue");
}

/**
 * The same target, but the proxy's get trap returns the REAL value.
 * Returns the value read.
 *
 * TODO: implement — a trap that agrees with the target is always allowed.
 */
export function agreeAboutValue() {
  throw new Error("TODO: implement agreeAboutValue");
}

/**
 * A target with a non-configurable property `fixed`, wrapped in a proxy
 * whose ownKeys trap returns []. Calls Object.getOwnPropertyNames on it
 * and returns the keys or the caught error's name.
 *
 * TODO: implement. A non-configurable key cannot be hidden.
 */
export function hideNonConfigurableKey() {
  throw new Error("TODO: implement hideNonConfigurableKey");
}

/**
 * A non-extensible target wrapped in a proxy whose isExtensible trap
 * returns true. Calls Object.isExtensible on the proxy and returns the
 * answer or the caught error's name.
 *
 * TODO: implement.
 */
export function lieAboutExtensible() {
  throw new Error("TODO: implement lieAboutExtensible");
}

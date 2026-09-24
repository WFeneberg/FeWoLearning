// Exercise 071 — Proxy basics (advanced).
// Goal:   intercept the operations an object usually handles itself.
// Drills: the get, set, has and deleteProperty traps; a trap that throws;
//         keeping the target intact behind the proxy.
// Passes: strict() turns a typo into a ReferenceError instead of
//         undefined, and hide() makes a key invisible to `in`, spread and
//         Object.keys alike.

/**
 * A view of `target` whose reads throw
 * ReferenceError(`unknown property: <key>`) for any key the target does
 * not have. Symbol keys are always allowed through — the runtime probes
 * objects with them (Symbol.toPrimitive, Symbol.iterator, …).
 *
 * TODO: implement with a get trap.
 */
export function strict(_target) {
  throw new Error("TODO: implement strict");
}

/**
 * A view of `target` whose writes are checked: for a key in `validators`,
 * the value must satisfy validators[key](value) or the write throws a
 * TypeError(`invalid value for <key>`). Unvalidated keys pass through.
 *
 * TODO: implement with a set trap. Remember the trap must return true for
 * an accepted write.
 */
export function validated(_target, _validators) {
  throw new Error("TODO: implement validated");
}

/**
 * { view, counts } where counts is a Map of key -> number of READS through
 * the view. Writes are not counted.
 *
 * TODO: implement.
 */
export function counting(_target) {
  throw new Error("TODO: implement counting");
}

/**
 * A view of `target` with `hiddenKeys` removed from every angle: reading
 * one gives undefined, `in` says false, Object.keys and spread skip it,
 * and deleting it is a no-op that still reports success.
 *
 * TODO: implement with get, has, ownKeys and getOwnPropertyDescriptor
 * traps. (ownKeys alone is not enough: Object.keys filters the result by
 * each key's descriptor.)
 */
export function hide(_target, _hiddenKeys) {
  throw new Error("TODO: implement hide");
}

// Exercise 072 — Reflect and the receiver (advanced).
// Goal:   forward a trap correctly, which `target[key]` does not.
// Drills: Reflect.get/set with the receiver, Reflect.ownKeys, Reflect.has,
//         and what a getter's `this` is when a proxy forwards badly.
// Passes: a getter that reads another property through `this` is itself
//         intercepted — which only happens when the receiver is passed on.

/**
 * Logs every read into `log` as a key name and forwards it, PASSING THE
 * RECEIVER — so a getter on the target that reads `this.other` goes
 * through the proxy again and logs "other" as well.
 *
 * TODO: implement with Reflect.get(target, key, receiver).
 */
export function logReads(_target, _log) {
  throw new Error("TODO: implement logReads");
}

/**
 * The same thing written the WRONG way, for contrast: forward with
 * `target[key]` and no receiver. A getter then runs with `this === target`
 * and its inner reads are invisible.
 *
 * TODO: implement exactly that.
 */
export function logReadsWithoutReceiver(_target, _log) {
  throw new Error("TODO: implement logReadsWithoutReceiver");
}

/**
 * A view whose reads of a MISSING key return `fallback` instead of
 * undefined. An existing key — including one whose value is undefined —
 * is returned as it is.
 *
 * TODO: implement with Reflect.has and Reflect.get.
 */
export function withDefault(_target, _fallback) {
  throw new Error("TODO: implement withDefault");
}

/**
 * Every own key of `target`, string and symbol alike, in the order the
 * runtime reports them.
 *
 * TODO: implement with Reflect.ownKeys.
 */
export function allOwnKeys(_target) {
  throw new Error("TODO: implement allOwnKeys");
}

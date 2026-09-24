// Exercise 019 — optional chaining (beginner).
// Goal:   read down a path that might not be there, without a pyramid of &&.
// Drills: `?.`, `?.[]`, `?.()`, short-circuiting, pairing with `??`.
// Passes: readKey() does not even EVALUATE its key when the source is
//         missing — short-circuit, not "call and ignore".

/**
 * user?.address?.city, falling back to "unknown" for a missing user,
 * missing address, missing city, or an explicit null anywhere on the path.
 *
 * TODO: implement.
 */
export function cityOf(_user) {
  throw new Error("TODO: implement cityOf");
}

/**
 * Calls `target.onEvent(payload)` if — and only if — such a method is
 * there, returning its result. Returns undefined otherwise. A `target` with
 * no `onEvent` is fine; an `onEvent` that is not callable is NOT, and must
 * still throw a TypeError.
 *
 * TODO: implement with `?.()`.
 */
export function notify(_target, _payload) {
  throw new Error("TODO: implement notify");
}

/**
 * Reads `source[keyFn()]` — but when `source` is null or undefined the
 * whole rest of the chain is skipped, so `keyFn` must NOT be called at all.
 *
 * TODO: implement with `?.[]`.
 */
export function readKey(_source, _keyFn) {
  throw new Error("TODO: implement readKey");
}

/**
 * The number of items in `order.items`, or 0 when order, items or length is
 * missing. Beware: `??` and `||` differ here for a legitimate 0.
 *
 * TODO: implement.
 */
export function itemCount(_order) {
  throw new Error("TODO: implement itemCount");
}

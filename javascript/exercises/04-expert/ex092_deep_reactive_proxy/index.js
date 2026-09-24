// Exercise 092 — a deep reactive proxy (expert).
// Goal:   what a reactivity library does on the read and write path.
// Drills: proxies created lazily per nested object, identity caching so
//         the same child always gives the same proxy, change paths,
//         arrays, and not notifying for a write that changes nothing.
// Passes: reading the same nested object twice returns the SAME proxy —
//         without that, every read allocates and no === comparison holds.

/**
 * Wraps `target` so every write anywhere below it calls
 * onChange({ path, value, previous }), where `path` is an array of keys
 * from the root.
 *
 *   - nested objects and arrays are wrapped lazily, on read
 *   - reading the same nested object twice gives the same proxy
 *   - a write of an equal value (SameValueZero) notifies nothing
 *   - deleting a key notifies with value undefined
 *   - an array push notifies for the new INDEX only: the "length" write
 *     push performs afterwards carries the value the array already has,
 *     and an unchanged write notifies nothing. Truncating with
 *     `list.length = 0` does notify.
 *   - the underlying target is really updated
 *
 * TODO: implement.
 */
export function reactive(_target, _onChange) {
  throw new Error("TODO: implement reactive");
}

/**
 * The raw object behind a reactive proxy — the escape hatch every such
 * library needs for passing data to code that must not trigger anything.
 * Returns the value unchanged if it is not one of ours.
 *
 * TODO: implement, e.g. with a well-known symbol the get trap answers.
 */
export function toRaw(_value) {
  throw new Error("TODO: implement toRaw");
}

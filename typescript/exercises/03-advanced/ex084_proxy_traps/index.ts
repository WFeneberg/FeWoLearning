// Exercise 084 — Proxy (advanced).
// Goal:   intercept property access on an object you hand out.
// Drills: the get, set and has traps, Reflect as the default behaviour,
//         and the invariants a proxy may not break.
// Passes: reads are recorded, writes are validated, `in` lies on purpose,
//         and the target is never corrupted.
//
// A Proxy wraps a target and lets a handler intervene on the fundamental
// operations. It is the most powerful thing in the language and the most
// easily misused, so two habits matter from the start.
//
// Delegate with REFLECT, not by hand. `Reflect.get(target, key, receiver)`
// does exactly what the default would, including the receiver handling
// that a getter on a prototype needs. Writing `target[key]` instead works
// until it does not.
//
// And the types do not help you. `new Proxy(target, handler)` is typed
// `T`, the target's type, whatever the handler actually does — so a
// proxy that adds members or changes their types is lying to the checker
// by construction. That is not a flaw to work around; it is the cost of
// the tool, and it is why a proxy belongs behind a narrow, hand-written
// signature rather than being handed out raw.
//
// That also makes this row runtime-graded: all three signatures are
// `(target: T, …) => T` and have to be, so a type fact over them is
// green before any work is done. One was written and deleted.
//
// An invariant worth knowing before it bites: a proxy may not report
// something false about a NON-CONFIGURABLE, non-writable property of its
// target. `get` returning a different value for such a property throws a
// TypeError at the access, not at the definition.

export interface Access {
  /** Keys that were read, in order, including repeats. */
  reads: string[];
}

/**
 * TODO: wrap `target` so every property READ appends the key to
 * `access.reads` and then returns the real value.
 */
export function recording<T extends object>(_target: T, _access: Access): T {
  throw new Error("TODO: implement recording");
}

/**
 * TODO: wrap `target` so a write is rejected with a TypeError unless
 * `isValid(key, value)` accepts it. A rejected write must leave the
 * target untouched.
 */
export function validated<T extends object>(
  _target: T,
  _isValid: (key: string, value: unknown) => boolean,
): T {
  throw new Error("TODO: implement validated");
}

/**
 * TODO: wrap `target` so `in` reports false for any key starting with
 * "_", while reading it still works. Everything else behaves normally.
 */
export function hidingUnderscored<T extends object>(_target: T): T {
  throw new Error("TODO: implement hidingUnderscored");
}

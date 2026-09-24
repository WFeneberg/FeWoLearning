// Exercise 047 — property descriptors (intermediate).
// Goal:   control what a property allows, not just what it holds.
// Drills: defineProperty, writable/enumerable/configurable, the fact that
//         defineProperty defaults all three to FALSE, and what a rejected
//         write does in strict mode.
// Passes: defineConstant() produces a property that throws on write, and
//         definedDefaults() reports three falses.

/**
 * Defines `key` as a visible constant: readable, shows up in Object.keys,
 * cannot be written, cannot be redefined or deleted. Returns the object.
 *
 * TODO: implement.
 */
export function defineConstant(_object, _key, _value) {
  throw new Error("TODO: implement defineConstant");
}

/**
 * Defines `key` as a normal, writable, deletable property that simply does
 * not show up in Object.keys, JSON.stringify or for..in. Returns the object.
 *
 * TODO: implement.
 */
export function defineHidden(_object, _key, _value) {
  throw new Error("TODO: implement defineHidden");
}

/**
 * The flags of an existing property as { writable, enumerable, configurable },
 * or null when the object has no such OWN property.
 *
 * TODO: implement.
 */
export function flagsOf(_object, _key) {
  throw new Error("TODO: implement flagsOf");
}

/**
 * Defines a property with `{ value: 1 }` and nothing else, and returns its
 * flags. All three default to false — unlike `object.key = 1`, which makes
 * all three true.
 *
 * Return { defined, assigned }, each an object of the three flags.
 *
 * TODO: implement.
 */
export function definedVsAssigned() {
  throw new Error("TODO: implement definedVsAssigned");
}

/**
 * Tries to write `value` to a non-writable `key` and returns the caught
 * error's name, or "no error" if the write was allowed.
 *
 * A module is strict-mode code, so the rejected write throws a TypeError
 * instead of failing silently.
 *
 * TODO: implement.
 */
export function writeToReadOnly(_object, _key, _value) {
  throw new Error("TODO: implement writeToReadOnly");
}

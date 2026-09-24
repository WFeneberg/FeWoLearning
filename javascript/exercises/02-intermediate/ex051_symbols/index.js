// Exercise 051 — symbols (intermediate).
// Goal:   keys that cannot collide, and the hooks the language reads.
// Drills: Symbol() vs Symbol.for(), symbol keys and reflection,
//         Symbol.toStringTag, Symbol.toPrimitive.
// Passes: hiddenKey() is invisible to Object.keys and JSON but findable
//         with getOwnPropertySymbols, and money() coerces three ways.

/**
 * Returns { same, equal, registeredSame, keyFor } for two symbols with the
 * SAME description:
 *   - same: Symbol("x") === Symbol("x")            -> false
 *   - equal: their descriptions are equal          -> true
 *   - registeredSame: Symbol.for("x") === Symbol.for("x") -> true
 *   - keyFor: Symbol.keyFor(Symbol.for("x"))       -> "x"
 *
 * TODO: implement.
 */
export function symbolIdentity() {
  throw new Error("TODO: implement symbolIdentity");
}

/**
 * Returns { object, key } where `object` carries a value under a SYMBOL key
 * and a "visible" string key, and `key` is that symbol — so a test can read
 * the value back.
 *
 * TODO: implement.
 */
export function hiddenKey(_value) {
  throw new Error("TODO: implement hiddenKey");
}

/**
 * An object whose Object.prototype.toString reads
 * "[object <tag>]" instead of "[object Object]".
 *
 * TODO: implement with Symbol.toStringTag.
 */
export function taggedObject(_tag) {
  throw new Error("TODO: implement taggedObject");
}

/**
 * A money object over `cents` with a Symbol.toPrimitive that answers:
 *   hint "number" -> the amount in whole units (cents / 100)
 *   hint "string" -> `<units> CHF`, two decimals
 *   hint "default" -> the same as "string"
 *
 * TODO: implement.
 */
export function money(_cents) {
  throw new Error("TODO: implement money");
}

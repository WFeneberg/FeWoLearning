// Exercise 070 — BigInt (intermediate).
// Goal:   exact integers past 2^53, and the walls around them.
// Drills: BigInt literals and conversion, truncating division, no mixing
//         with Number, loose vs strict comparison, JSON's refusal.
// Passes: factorial(21n) is exact, where the Number version is already
//         wrong — and every failure mode here is reported rather than
//         hidden.

/**
 * n! as a BigInt. factorial(0n) is 1n.
 *
 * TODO: implement.
 */
export function factorial(_n) {
  throw new Error("TODO: implement factorial");
}

/**
 * BigInt division truncates towards zero rather than producing a fraction:
 * divide(7n, 2n) -> 3n, divide(-7n, 2n) -> -3n.
 *
 * TODO: implement.
 */
export function divide(_a, _b) {
  throw new Error("TODO: implement divide");
}

/**
 * Tries `1n + 1` and returns the caught error's name — "TypeError". BigInt
 * and Number never mix in arithmetic, deliberately: the result's precision
 * would be undefined.
 *
 * TODO: implement.
 */
export function mixingError() {
  throw new Error("TODO: implement mixingError");
}

/**
 * { loose, strict, greater } for 1n vs 1 and 2n vs 1:
 *   loose:   1n == 1    -> true   (comparison DOES convert)
 *   strict:  1n === 1   -> false  (different types)
 *   greater: 2n > 1     -> true
 *
 * TODO: implement.
 */
export function comparisons() {
  throw new Error("TODO: implement comparisons");
}

/**
 * Serializes an object containing a BigInt by writing it as a decimal
 * STRING — JSON.stringify throws a TypeError on a BigInt otherwise.
 *
 * serializeBig({ id: 1n }) -> '{"id":"1"}'
 *
 * TODO: implement with a replacer.
 */
export function serializeBig(_value) {
  throw new Error("TODO: implement serializeBig");
}

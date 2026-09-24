// Exercise 001 — value types (beginner).
// Goal:   classify any value the way the runtime actually sees it.
// Drills: `typeof`, the seven primitive types, why `typeof null` lies,
//         Array.isArray, primitive vs object.
// Passes: describeValue() names every category, and isPrimitive() answers
//         without listing object types.
//
// Coming from C#: there is no `GetType()`. `typeof` returns one of eight
// strings, and two of them are famous potholes — `typeof null` is "object"
// (a bug from 1995 that can never be fixed), and every array, Date and Map
// is also just "object".

/**
 * Returns one of: "string" | "number" | "bigint" | "boolean" | "symbol" |
 * "undefined" | "function" | "null" | "array" | "object".
 *
 * TODO: implement. `typeof` gets you most of the way; null and arrays need
 * their own checks first.
 */
export function describeValue(_value) {
  throw new Error("TODO: implement describeValue");
}

/**
 * True for the seven primitive types (string, number, bigint, boolean,
 * symbol, undefined, null), false for everything else.
 *
 * TODO: implement. Do not enumerate object types — there is no end to them.
 */
export function isPrimitive(_value) {
  throw new Error("TODO: implement isPrimitive");
}

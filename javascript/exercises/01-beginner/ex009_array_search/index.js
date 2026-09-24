// Exercise 009 — searching an array (beginner).
// Goal:   pick the right one of the seven search methods.
// Drills: find, findLast, findIndex, some, every, includes vs indexOf.
// Passes: every function reports the documented answer for the empty list
//         too — and containsValue() finds a NaN, which indexOf never can.

/** The first user with this id, or undefined. TODO: implement. */
export function findUser(_users, _id) {
  throw new Error("TODO: implement findUser");
}

/**
 * The LAST event whose level is "error", or undefined. Do not reverse a
 * copy — there is a method for this.
 *
 * TODO: implement.
 */
export function lastError(_events) {
  throw new Error("TODO: implement lastError");
}

/** True if at least one user has role "admin". TODO: implement. */
export function hasAdmin(_users) {
  throw new Error("TODO: implement hasAdmin");
}

/**
 * True if every user is active. Note what this answers for an empty list:
 * `every` is true over nothing, `some` is false over nothing.
 *
 * TODO: implement.
 */
export function allActive(_users) {
  throw new Error("TODO: implement allActive");
}

/**
 * True if `value` occurs in the list, comparing with SameValueZero — so a
 * NaN in the list IS found, unlike with indexOf.
 *
 * TODO: implement.
 */
export function containsValue(_list, _value) {
  throw new Error("TODO: implement containsValue");
}

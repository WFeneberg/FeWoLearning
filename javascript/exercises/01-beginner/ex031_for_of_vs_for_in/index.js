// Exercise 031 — for..of vs for..in (beginner).
// Goal:   never write for..in over an array again.
// Drills: for..of over any iterable, for..in over enumerable string keys
//         INCLUDING inherited ones, .entries() for index plus value.
// Passes: keysViaForIn() picks up a prototype's enumerable property, and
//         indexKeysOfArray() shows that array indices are strings.

/**
 * Every value of any iterable, as an array — using a for..of loop rather
 * than spread, so the loop itself is what is being practised.
 *
 * TODO: implement.
 */
export function valuesViaForOf(_iterable) {
  throw new Error("TODO: implement valuesViaForOf");
}

/**
 * Every key a for..in loop visits, in order: the object's own enumerable
 * string keys AND the enumerable ones it inherits.
 *
 * TODO: implement with for..in.
 */
export function keysViaForIn(_object) {
  throw new Error("TODO: implement keysViaForIn");
}

/**
 * What a for..in loop yields for an array: the INDICES, as strings, plus
 * any other own enumerable property the array happens to carry.
 *
 * TODO: implement with for..in.
 */
export function indexKeysOfArray(_list) {
  throw new Error("TODO: implement indexKeysOfArray");
}

/**
 * [[index, value], …] for a list, via for..of over .entries().
 *
 * TODO: implement.
 */
export function indexedPairs(_list) {
  throw new Error("TODO: implement indexedPairs");
}

// Exercise 002 — equality and coercion (beginner).
// Goal:   know which of the four equality algorithms you are using.
// Drills: `==` vs `===`, `Object.is`, SameValueZero, NaN, -0.
// Passes: isSameValueZero() matches what Set and includes() do, dedupe()
//         collapses NaN, and normalizeZero() can tell -0 from 0.
//
// JavaScript has four: loose (`==`, with coercion), strict (`===`),
// SameValue (`Object.is`) and SameValueZero (what Set, Map keys and
// Array#includes use). They differ on exactly two values — NaN and -0.

/**
 * SameValueZero: like `===`, except NaN equals NaN. +0 and -0 are equal.
 *
 * TODO: implement without calling Object.is, Set or includes — the point is
 * to spell the algorithm out.
 */
export function isSameValueZero(_a, _b) {
  throw new Error("TODO: implement isSameValueZero");
}

/**
 * Returns a new array with duplicates removed, comparing with SameValueZero,
 * keeping first-seen order. A single NaN survives; +0 and -0 collapse.
 *
 * TODO: implement.
 */
export function dedupe(_values) {
  throw new Error("TODO: implement dedupe");
}

/**
 * Returns 0 for -0 and leaves every other number alone (NaN included).
 * `-0 === 0` is true, so `===` cannot see the difference — something else can.
 *
 * TODO: implement.
 */
export function normalizeZero(_value) {
  throw new Error("TODO: implement normalizeZero");
}

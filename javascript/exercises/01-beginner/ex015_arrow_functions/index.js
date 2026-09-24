// Exercise 015 — arrow functions (beginner).
// Goal:   use the short form, and understand the one real difference.
// Drills: concise bodies, returning an object literal, closing over a
//         parameter, an arrow keeping `this` inside a callback.
// Passes: makeTagger() returns a working closure, makeRecord() returns an
//         object rather than undefined, and total() sees `this`.
//
// An arrow is not "shorter function". It has no `this`, no `arguments`, no
// `prototype` and cannot be `new`ed — it borrows `this` from where it was
// WRITTEN, which is exactly what a callback usually wants.

/** double(4) -> 8. TODO: implement as a one-expression arrow. */
export const double = (_n) => {
  throw new Error("TODO: implement double");
};

/**
 * makeTagger("dev")("api") -> "[dev] api"
 *
 * TODO: implement as an arrow returning an arrow.
 */
export const makeTagger = (_tag) => {
  throw new Error("TODO: implement makeTagger");
};

/**
 * makeRecord(1) -> { id: 1, ok: true }
 *
 * TODO: implement with a CONCISE body. An object literal needs parentheses
 * there, or the braces read as a function body and the result is undefined.
 */
export const makeRecord = (_id) => {
  throw new Error("TODO: implement makeRecord");
};

/**
 * Returns a cart object { rate, items: [], add(price), total() }.
 * `add` pushes a price and returns the cart, so calls chain. `total()`
 * returns the sum of every price multiplied by the cart's own `rate` —
 * computed inside a reduce callback, which therefore has to see `this`.
 * A `function () {}` callback there would not.
 *
 * TODO: implement. `total` is a method, not a getter.
 */
export function makeCart(_rate = 1) {
  throw new Error("TODO: implement makeCart");
}

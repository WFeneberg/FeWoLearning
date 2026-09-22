// Exercise 039 — conditional types (intermediate).
// Goal:   branch in the type system on whether one type fits another.
// Drills: `T extends U ? X : Y`, chaining conditionals, checking for a
//         member rather than a name.
// Passes: Classify sorts four kinds of type, and HasLength answers by
//         shape. classifyValue does the same job at runtime.
//
// `extends` here is not inheritance. It reads "is assignable to", which in
// a structural system means "has at least this shape" — so
// `{ a: 1; b: 2 } extends { a: number }` is true, and there is nothing to
// declare anywhere. It is the type-level `if`.
//
// Order matters, because the arms are tried top to bottom and almost
// everything is an `object`. Arrays and functions are objects too, so they
// have to be asked about first.
//
// One caveat deferred to ex046: when the checked type is a bare type
// parameter and the argument is a union, the conditional DISTRIBUTES over
// that union rather than answering once. Every fact below passes a
// non-union, so it does not bite yet.

/** TODO: "array" for any array, "function" for any function, "object" for
 *  anything else object-shaped, and "primitive" otherwise. */
export type Classify<T> = unknown;

/** TODO: true when T has a numeric `length`, false otherwise. */
export type HasLength<T> = unknown;

/** The same four-way split, at runtime. */
export function classifyValue(
  _value: unknown,
): "array" | "function" | "object" | "primitive" {
  throw new Error("TODO: implement classifyValue");
}

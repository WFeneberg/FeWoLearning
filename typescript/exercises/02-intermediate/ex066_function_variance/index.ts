// Exercise 066 — variance, and the hole left in it on purpose
// (intermediate).
// Goal:   work out which way function types are assignable, and find the
//         one place the rule is suspended.
// Drills: parameter contravariance under strictFunctionTypes, return
//         covariance, method bivariance.
// Passes: the three probes answer differently, which is the row.
//
// A handler is assignable to another when it asks for LESS and gives back
// MORE. Parameters go contravariantly: a `(a: Animal) => void` can stand
// in wherever a `(a: Dog) => void` is wanted, because anything handing it
// a Dog is handing it an Animal. The reverse is unsound — a Dog handler
// would read `.breed` off a plain Animal — and `strictFunctionTypes`,
// which `strict` turns on, rejects it.
//
// THE HOLE: `strictFunctionTypes` applies only to function types written
// in PROPERTY position. A member written with method syntax —
// `handle(a: Dog): void` rather than `handle: (a: Dog) => void` — stays
// BIVARIANT, assignable in both directions, unsound and deliberate. It is
// there because `Array<Dog>` must remain assignable to `Array<Animal>`,
// and every array method would otherwise stand in the way. Two members
// that behave identically at runtime therefore check differently, purely
// because of how they were spelled.

export interface Animal {
  name: string;
}

export interface Dog extends Animal {
  name: string;
  breed: string;
}

/** TODO: true when a handler taking A can stand in for one taking B. */
export type HandlerAssignable<A, B> = unknown;

/** TODO: true when an object whose `handle` PROPERTY takes A can stand in
 *  for one whose `handle` property takes B. */
export type PropertyAssignable<A, B> = unknown;

/** TODO: the same question for a `handle` METHOD. Write both sides with
 *  method syntax. */
export type MethodAssignable<A, B> = unknown;

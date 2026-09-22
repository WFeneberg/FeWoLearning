// Exercise 012 — `unknown` is the honest `any` (beginner).
// Goal:   accept a value of no known type without giving up checking.
// Drills: unknown as a parameter, narrowing before use, unknown as a return.
// Passes: parseJson reports unknown rather than any, and both readers narrow
//         before touching the value.
//
// `any` switches the checker off for everything downstream of it; `unknown`
// keeps it on and simply refuses access until you have proved something. The
// practical consequence is at the edges of a program: JSON.parse is typed to
// return `any`, so its result silently infects whatever it flows into unless
// you stop it.
//
// parseJson has NO return type annotation on purpose: the graded type is the
// one your body produces, so returning JSON.parse's result unchanged reports
// `any` and fails the fact.

/** Parses JSON text. TODO: the result must be unknown, not any. */
export function parseJson(_text: string) {
  throw new Error("TODO: implement parseJson");
}

/** The length of a string or array; 0 for anything else. */
export function lengthOf(_value: unknown): number {
  throw new Error("TODO: implement lengthOf");
}

/** The value as a plain object of unknown values, or undefined if it is not one. */
export function asRecord(_value: unknown): Record<string, unknown> | undefined {
  throw new Error("TODO: implement asRecord");
}

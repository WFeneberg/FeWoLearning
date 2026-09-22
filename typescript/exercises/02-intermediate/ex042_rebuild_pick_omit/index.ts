// Exercise 042 — rebuilding Pick and Omit, and fixing one of them
// (intermediate).
// Goal:   select and remove keys, and notice a real weakness in the
//         standard library while doing it.
// Drills: a mapped type over a key union, filtering with `as`, choosing a
//         constraint.
// Passes: MyPick and MyOmit produce the right shapes, MyOmit rejects a key
//         its source does not have, and omitAt does the same at runtime.
//
// Pick is `{ [P in K]: T[P] }` with `K extends keyof T`. Omit is the
// complement, and the standard library writes it as
// `Pick<T, Exclude<keyof T, K>>`.
//
// THE WEAKNESS: the built-in Omit constrains K to `keyof any`, not to
// `keyof T`. So `Omit<Row, "coutn">` — a typo — compiles happily and
// silently omits nothing. That is a deliberate choice by the library
// authors (it keeps Omit usable over unions), and it is also a bug factory.
// Your MyOmit constrains K to `keyof T` instead, and a fact below checks
// that the typo is rejected. The stub's constraint is the loose one, so
// tightening it is part of the work.

export interface Row {
  readonly id: string;
  label?: string;
  count: number;
}

/** TODO: only the keys in K, modifiers preserved. */
export type MyPick<T, K extends keyof T> = unknown;

/** TODO: everything except the keys in K — and tighten the constraint so a
 *  key that is not in T is a compile error. */
export type MyOmit<T, K extends keyof any> = unknown;

/** TODO: a copy of `row` without the named keys. */
export function omitAt<K extends keyof Row>(_row: Row, _keys: readonly K[]): MyOmit<Row, K> {
  throw new Error("TODO: implement omitAt");
}

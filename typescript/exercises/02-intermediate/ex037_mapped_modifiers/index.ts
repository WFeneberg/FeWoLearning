// Exercise 037 — adding and stripping modifiers (intermediate).
// Goal:   turn readonly and optional on and off across a whole type.
// Drills: `-readonly`, `-?`, `+readonly`, `+?`, and what a plain mapped
//         type does to modifiers all by itself.
// Passes: Same preserves what Row declares, Solid strips both modifiers,
//         and Loose adds both.
//
// The surprise is `Same` below. A mapped type of the form
// `{ [K in keyof T]: T[K] }` — where the source is exactly `keyof T` — is
// HOMOMORPHIC, and it copies `readonly` and `?` across for free. It is not
// an identity by accident; it is an identity by design, and it is why
// Partial and Readonly can be one line each.
//
// `-` removes a modifier and `+` adds one (`+` is the default, so
// `+readonly` and `readonly` mean the same thing — spell it out where it
// aids the reader). Note that `-?` removes `| undefined` from the value
// type as well, which is the whole reason Required works.

export interface Row {
  readonly id: string;
  label?: string;
  count: number;
}

/** TODO: an identity mapped type. Write the plain form and look at what
 *  comes out — you are not meant to add anything to preserve the modifiers. */
export type Same<T> = unknown;

/** TODO: every property mutable and required. */
export type Solid<T> = unknown;

/** TODO: every property readonly and optional. */
export type Loose<T> = unknown;

/** TODO: `row` with `label` defaulted to "" so nothing is optional. */
export function solidify(_row: Row): Solid<Row> {
  throw new Error("TODO: implement solidify");
}

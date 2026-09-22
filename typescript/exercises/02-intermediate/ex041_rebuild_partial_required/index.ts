// Exercise 041 — rebuilding Partial and Required (intermediate).
// Goal:   discover that two of the most-used library types are one line of
//         ex037 each.
// Drills: `?` and `-?` in a mapped type, and a per-key variant the standard
//         library does not provide.
// Passes: MyPartial and MyRequired match the built-ins, and PartialBy makes
//         exactly the named keys optional.
//
// lib.es5.d.ts really does define Partial as `{ [P in keyof T]?: T[P] }` and
// Required as `{ [P in keyof T]-?: T[P] }`. Nothing is built in about them.
//
// Note what `-?` does beyond removing the question mark: it also strips
// `| undefined` from the value type. That is deliberate and it is why
// Required<{ a?: string }> is `{ a: string }` rather than
// `{ a: string | undefined }`.
//
// AN HONEST LIMIT, and it applies to every row from here to ex045: a fact
// asserts the resulting type and cannot see how you got there, so
// `type MyPartial<T> = Partial<T>` passes. That is why each of these rows
// also carries something the standard library has no answer for — here,
// PartialBy. Delegating is not available for those.

export interface Row {
  readonly id: string;
  label?: string;
  count: number;
}

/** TODO: every property optional. */
export type MyPartial<T> = unknown;

/** TODO: every property required, `| undefined` removed with it. */
export type MyRequired<T> = unknown;

/** TODO: only the keys named in K optional; everything else untouched. */
export type PartialBy<T, K extends keyof T> = unknown;

// Exercise 077 — parse, do not validate (advanced).
// Goal:   make a value whose type is evidence that it was checked.
// Drills: a smart constructor, the assertion form, and a brand nobody
//         else can mint.
// Passes: both constructors reject what is invalid, the branded results
//         flow where the raw types cannot, and the assertion narrows.
//
// ex076 built the brand. This row is what to do with it.
//
// A validator returning `boolean` throws its work away: the caller has a
// string afterwards exactly as it did before, and the next function down
// has to check again — or not, and that is the bug. A SMART CONSTRUCTOR
// returns the branded type instead, so the check happens once and the
// type carries the evidence. "Parse, don't validate": make the illegal
// state unrepresentable rather than merely unwelcome.
//
// The brand key here is module-local and never exported, so no other file
// can write the object type out. A caller can still reach for `as`, and
// that is the point — it becomes a deliberate, greppable act in one place
// rather than something that happens by accident everywhere.
//
// Both forms are here because they suit different call sites: the
// Option-shaped one for a boundary that must report, the assertion for
// code that would rather fail loudly.

declare const brand: unique symbol;

type Brand<T, B extends string> = T & { readonly [brand]: B };

export type Email = Brand<string, "Email">;

export type PositiveInt = Brand<number, "PositiveInt">;

/** TODO: an Email when `value` has exactly one "@" with something either
 *  side and no whitespace, undefined otherwise. */
export function toEmail(_value: string): Email | undefined {
  throw new Error("TODO: implement toEmail");
}

/** TODO: a PositiveInt when `value` is an integer greater than zero,
 *  undefined otherwise. Reject NaN and Infinity too. */
export function toPositiveInt(_value: number): PositiveInt | undefined {
  throw new Error("TODO: implement toPositiveInt");
}

/** TODO: the assertion form — throw a TypeError, and narrow. */
export function assertEmail(_value: string): void {
  throw new Error("TODO: implement assertEmail");
}

/** TODO: `<email> x<count>`. Accepts only values that were checked. */
export function describeQuota(_email: Email, _count: PositiveInt): string {
  throw new Error("TODO: implement describeQuota");
}

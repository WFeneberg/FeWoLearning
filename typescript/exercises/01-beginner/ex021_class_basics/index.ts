// Exercise 021 — classes, fields and access modifiers (beginner).
// Goal:   build a small class whose invariants the type system defends.
// Drills: parameter properties, readonly fields, private fields.
// Passes: id is a readonly string, balance is no longer part of the public
//         type, and the three operations behave.
//
// The stub declares both fields as writable, public and `unknown`. Tightening
// them is the exercise — and note what "private" means here: `private` is
// erased at compile time, so the field is perfectly readable at runtime from
// plain JavaScript. It is a statement about your own code, not a protection.
// ex025 covers the `#` form, which is real.
//
// TypeScript's parameter properties — `constructor(public readonly id: string)`
// — declare, accept and assign a field in one place, with no C# equivalent
// before primary constructors. Use them. Be aware, though, that no test can
// tell a parameter property from a field plus an assignment in the body: the
// resulting class type is identical, so the facts below grade the field's
// type and modifiers, never the syntax that produced them.

export class Account {
  /** TODO: a public, readonly string. */
  id: unknown = undefined;

  /** TODO: a private number. */
  balance: unknown = undefined;

  constructor(_id: string, _initialBalance: number) {
    throw new Error("TODO: implement the Account constructor");
  }

  /** The current balance. */
  getBalance(): number {
    throw new Error("TODO: implement getBalance");
  }

  /** Adds to the balance. */
  deposit(_amount: number): void {
    throw new Error("TODO: implement deposit");
  }

  /** Subtracts if there is enough; returns whether it happened. */
  withdraw(_amount: number): boolean {
    throw new Error("TODO: implement withdraw");
  }
}

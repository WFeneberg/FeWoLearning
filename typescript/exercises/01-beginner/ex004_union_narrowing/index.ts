// Exercise 004 — narrowing a union (beginner).
// Goal:   turn one union-typed parameter into four type-safe branches.
// Drills: typeof narrowing, Array.isArray, control-flow analysis.
// Passes: describe() reports the right tag and payload for all four members.
//
// Two traps this row exists for: `typeof []` is "object", not "array", so
// arrays need Array.isArray; and narrowing is a property of the control flow,
// so it does not survive into a callback that runs later.
//
// This row is graded by runtime facts only. Its whole subject is what the
// checker infers INSIDE a branch, and a test asserting the declared parameter
// or return type would be satisfied by the stub's signature before the body
// ever runs.

export type Input = string | number | boolean | string[];

/**
 * TODO: return
 *   `text:<value>`     for a string,
 *   `number:<value>`   for a number,
 *   `boolean:<value>`  for a boolean,
 *   `list:<length>`    for a string array.
 */
export function describe(_input: Input): string {
  throw new Error("TODO: implement describe");
}

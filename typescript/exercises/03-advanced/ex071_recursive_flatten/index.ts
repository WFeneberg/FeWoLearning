// Exercise 071 — a conditional type that calls itself (advanced).
// Goal:   write the first genuinely recursive type and see where it stops.
// Drills: recursion in a conditional, `infer` at each level, the base case.
// Passes: Flatten reaches the innermost element type at any depth,
//         FlattenOnce stops after one, and flattenDeep does the runtime
//         half.
//
// A conditional type may refer to itself. `Flatten<T>` asks whether T is
// an array; if it is, it asks the same question of the element type, and
// keeps going until the answer is no. The base case is not a special
// construct — it is simply the branch that does not recurse.
//
// TypeScript permits this up to an instantiation-depth limit, which is
// generous for a chain like this one and easy to exceed with a naive
// accumulation (ex094, ex095). A type recursing on something strictly
// SMALLER each step, as this one does, is never the problem.
//
// FlattenOnce is there for contrast: the two differ by one word, and the
// facts pin down exactly what that word buys.
//
// Note that a string is not an array here even though it is indexable —
// `string extends readonly unknown[]` is false, so text comes through as
// itself rather than dissolving into characters.

/** TODO: the innermost element type, however deeply nested. */
export type Flatten<T> = unknown;

/** TODO: one level only. */
export type FlattenOnce<T> = unknown;

/** TODO: flatten a nested array completely, keeping order. */
export function flattenDeep(_value: readonly unknown[]): unknown[] {
  throw new Error("TODO: implement flattenDeep");
}

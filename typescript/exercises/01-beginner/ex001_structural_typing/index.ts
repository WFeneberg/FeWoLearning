// Exercise 001 — structural typing (beginner).
// Goal:   see that TypeScript compares shapes, not declared names.
// Drills: structural assignability, a class satisfying an interface it never
//         names, conditional types as an assignability probe.
// Passes: greet() accepts anything with a string `name`, and IsNamed<S>
//         answers true/false by shape alone.
//
// Coming from C#: there is no `implements` to write and none to look for. A
// type is a description of a shape, and every value of that shape belongs to
// it. Nominality has to be built on purpose — see ex076_branded_types.

export interface Named {
  name: string;
}

/** Returns `Hello, <name>!`. */
export function greet(_target: Named): string {
  throw new Error("TODO: implement greet");
}

/**
 * TODO: resolve to `true` when S is structurally assignable to Named,
 * and to `false` otherwise.
 */
export type IsNamed<S> = unknown;

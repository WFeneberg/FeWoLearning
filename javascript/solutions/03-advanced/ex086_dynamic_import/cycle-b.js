// Support module for exercise 086 — NOT a TODO, identical in both trees.
// The other half of the cycle. This module is evaluated FIRST (cycle-a
// imports it before its own body runs), so everything it reads from
// cycle-a here is read from a module that has not finished evaluating.
import { fromA, A_CONST } from "./cycle-a.js";

// A function declaration is hoisted and initialised before evaluation, so
// this is "function" even mid-cycle.
export const B_SAW_A_AS = typeof fromA;

// A `const` is NOT yet usable: its own line has not run. On plain Node
// that is a ReferenceError (the temporal dead zone); under Vitest's module
// runner, which rewrites ESM into its own runtime, the same read yields
// undefined. Either way it is not the finished value, which is the part
// worth knowing — and the part ex086 grades.
export const B_SAW_CONST = (() => {
  try {
    return A_CONST;
  } catch (error) {
    return error.name;
  }
})();

export function fromB() {
  return "b";
}

// Support module for exercise 086 — NOT a TODO, identical in both trees.
// One half of a deliberate import cycle. See cycle-b.js.
import { fromB, B_SAW_A_AS, B_SAW_CONST } from "./cycle-b.js";

export const A_CONST = "a-const";

export function fromA() {
  return "a";
}

export const report = {
  // What B could see of A while A was still evaluating.
  bSawFunction: B_SAW_A_AS,
  bSawConst: B_SAW_CONST,
  // And what A sees of B, which finished first.
  aSeesB: typeof fromB,
};

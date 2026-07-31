// Exercise 004 — useFullName composable (beginner).
// Goal:   a `fullName` computed derived from `firstName` and `lastName` refs.
// Drills: ref, computed, deriving state from multiple reactive sources.
import { type Ref } from "vue";

export interface FullName {
  firstName: Ref<string>;
  lastName: Ref<string>;
  fullName: Ref<string>;
}

export function useFullName(_first = "", _last = ""): FullName {
  throw new Error("TODO: implement useFullName");
}

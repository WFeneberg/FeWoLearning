// Exercise 035 — useSortedNames composable (beginner).
// Goal:   a computed that returns a sorted copy of a names array.
// Drills: computed, non-mutating array sort, ref arrays.
import { type ComputedRef, type Ref } from "vue";

export interface SortedNames {
  names: Ref<string[]>;
  sortedNames: ComputedRef<string[]>;
}

export function useSortedNames(_initial: string[]): SortedNames {
  throw new Error("TODO: implement useSortedNames");
}

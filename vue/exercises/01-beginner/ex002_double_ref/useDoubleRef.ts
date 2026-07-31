// Exercise 002 — useDoubleRef composable (beginner).
// Goal:   a reactive counter whose `double()` function returns count * 2.
// Drills: ref, reading/writing .value, deriving a value from a ref.
import { ref, type Ref } from "vue";

export interface DoubleRef {
  count: Ref<number>;
  double: () => number;
}

export function useDoubleRef(_initial = 0): DoubleRef {
  throw new Error("TODO: implement useDoubleRef");
}

// Exercise 001 — useCounter composable (beginner).
// Goal:   a reactive counter with increment/decrement/reset.
// Drills: ref, returning a reactive API from a composable.
import { type Ref } from "vue";

export interface Counter {
  count: Ref<number>;
  increment: () => void;
  decrement: () => void;
  reset: () => void;
}

export function useCounter(_initial = 0): Counter {
  throw new Error("TODO: implement useCounter");
}

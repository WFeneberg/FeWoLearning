// Exercise 001 — useCounter composable (reference solution).
import { ref, type Ref } from "vue";

export interface Counter {
  count: Ref<number>;
  increment: () => void;
  decrement: () => void;
  reset: () => void;
}

export function useCounter(initial = 0): Counter {
  const count = ref(initial);
  return {
    count,
    increment: () => {
      count.value += 1;
    },
    decrement: () => {
      count.value -= 1;
    },
    reset: () => {
      count.value = initial;
    },
  };
}

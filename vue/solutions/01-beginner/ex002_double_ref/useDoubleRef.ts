// Exercise 002 — useDoubleRef composable (reference solution).
import { ref, type Ref } from "vue";

export interface DoubleRef {
  count: Ref<number>;
  double: () => number;
}

export function useDoubleRef(initial = 0): DoubleRef {
  const count = ref(initial);
  return {
    count,
    double: () => count.value * 2,
  };
}

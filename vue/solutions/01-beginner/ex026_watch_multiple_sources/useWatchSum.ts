// Exercise 026 — useWatchSum composable (reference solution).
import { ref, watch, type Ref } from "vue";

export interface WatchSum {
  a: Ref<number>;
  b: Ref<number>;
  sums: Ref<number[]>;
}

export function useWatchSum(initialA = 0, initialB = 0): WatchSum {
  const a = ref(initialA);
  const b = ref(initialB);
  const sums = ref<number[]>([]);

  watch([a, b], ([newA, newB]) => {
    sums.value.push(newA + newB);
  });

  return { a, b, sums };
}

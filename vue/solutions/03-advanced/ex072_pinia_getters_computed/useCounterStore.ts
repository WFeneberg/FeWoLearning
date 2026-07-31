// Exercise 072 — Pinia getters (reference solution).
// A Pinia setup store's `state` is a `ref`, its `getters` are `computed()`
// over that state (and may chain off one another), and its `actions` are
// plain functions that mutate the state — which is exactly what `defineStore`
// compiles down to. Building it directly on Vue's reactivity primitives here
// keeps the exercise dependency-free while exercising the identical
// reactive behavior `defineStore(...).doubleCount` would have.
import { computed, ref, type ComputedRef, type Ref } from "vue";

export interface CounterStore {
  count: Ref<number>;
  doubleCount: ComputedRef<number>;
  quadrupleCount: ComputedRef<number>;
  increment: (amount?: number) => void;
}

export function useCounterStore(initial = 0): CounterStore {
  // state
  const count = ref(initial);

  // getters
  const doubleCount = computed(() => count.value * 2);
  const quadrupleCount = computed(() => doubleCount.value * 2);

  // actions
  function increment(amount = 1) {
    count.value += amount;
  }

  return { count, doubleCount, quadrupleCount, increment };
}

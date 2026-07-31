// Exercise 026 — useWatchSum composable (beginner).
// Goal:   watch an array of sources and record derived values.
// Drills: watch([a, b], callback), multi-source watchers.
import { ref, type Ref } from "vue";

export interface WatchSum {
  a: Ref<number>;
  b: Ref<number>;
  sums: Ref<number[]>;
}

export function useWatchSum(initialA = 0, initialB = 0): WatchSum {
  throw new Error("TODO: implement useWatchSum");
}

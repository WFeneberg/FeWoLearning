// Exercise 005 — useWatchBasic composable (reference solution).
import { ref, watch, type Ref } from "vue";

export interface WatchBasic {
  count: Ref<number>;
  history: [number, number][];
}

export function useWatchBasic(initial = 0): WatchBasic {
  const count = ref(initial);
  const history: [number, number][] = [];

  watch(count, (newVal, oldVal) => {
    history.push([oldVal, newVal]);
  });

  return { count, history };
}

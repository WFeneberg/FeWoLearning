// Exercise 005 — useWatchBasic composable (beginner).
// Goal:   watch a single ref and record old/new value pairs.
// Drills: watch, side effects, accumulating history in a plain array.
import { ref, type Ref } from "vue";

export interface WatchBasic {
  count: Ref<number>;
  history: [number, number][];
}

export function useWatchBasic(_initial = 0): WatchBasic {
  throw new Error("TODO: implement useWatchBasic");
}

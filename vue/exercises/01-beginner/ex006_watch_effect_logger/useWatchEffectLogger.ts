// Exercise 006 — useWatchEffectLogger composable (beginner).
// Goal:   a watchEffect that auto-tracks two refs and logs whenever either changes.
// Drills: watchEffect, automatic dependency tracking, side effects.
import { type Ref } from "vue";

export interface WatchEffectLogger {
  name: Ref<string>;
  age: Ref<number>;
  log: string[];
}

export function useWatchEffectLogger(initialName = "Anna", initialAge = 30): WatchEffectLogger {
  throw new Error("TODO: implement useWatchEffectLogger");
}

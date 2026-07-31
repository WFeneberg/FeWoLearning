// Exercise 006 — useWatchEffectLogger composable (reference solution).
import { ref, watchEffect, type Ref } from "vue";

export interface WatchEffectLogger {
  name: Ref<string>;
  age: Ref<number>;
  log: string[];
}

export function useWatchEffectLogger(initialName = "Anna", initialAge = 30): WatchEffectLogger {
  const name = ref(initialName);
  const age = ref(initialAge);
  const log: string[] = [];

  watchEffect(() => {
    log.push(`${name.value} is ${age.value}`);
  });

  return { name, age, log };
}

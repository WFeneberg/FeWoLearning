// Exercise 065 — useDebouncedRef composable (reference solution).
import { ref, type Ref } from "vue";

export interface DebouncedRef<T> {
  value: Ref<T>;
  set: (newValue: T) => void;
}

export function useDebouncedRef<T>(initial: T, delay: number): DebouncedRef<T> {
  const value = ref(initial) as Ref<T>;
  let timer: ReturnType<typeof setTimeout> | undefined;

  const set = (newValue: T) => {
    if (timer !== undefined) {
      clearTimeout(timer);
    }
    timer = setTimeout(() => {
      value.value = newValue;
      timer = undefined;
    }, delay);
  };

  return { value, set };
}

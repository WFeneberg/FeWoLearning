// Exercise 028 — useStorage composable (reference solution).
import { ref, watch, type Ref } from "vue";

export interface StorageLike {
  getItem: (key: string) => string | null;
  setItem: (key: string, value: string) => void;
}

export function useStorage<T>(
  key: string,
  defaultValue: T,
  store: StorageLike,
): Ref<T> {
  const stored = store.getItem(key);
  const initial = stored === null ? defaultValue : (JSON.parse(stored) as T);

  const value = ref(initial) as Ref<T>;

  watch(
    value,
    (newValue) => {
      store.setItem(key, JSON.stringify(newValue));
    },
    { flush: "sync" },
  );

  return value;
}

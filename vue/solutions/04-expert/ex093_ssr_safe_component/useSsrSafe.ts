// Exercise 093 — SSR-safe component primitives (reference solution).
import { getCurrentInstance, onMounted, ref, type Ref } from "vue";

const counters = new Map<string, number>();

export function resetIdCounter(): void {
  counters.clear();
}

export function useSsrSafeId(prefix: string): string {
  const next = (counters.get(prefix) ?? 0) + 1;
  counters.set(prefix, next);
  return `${prefix}-${next}`;
}

export function useClientOnly<T>(getter: () => T, fallback: T): Ref<T> {
  const value = ref(fallback) as Ref<T>;

  // onMounted only fires in a browser, and only inside a component instance.
  // Guarding on the instance keeps the composable usable (and silent) outside
  // one, instead of warning about a lifecycle hook with no owner.
  if (getCurrentInstance()) {
    onMounted(() => {
      value.value = getter();
    });
  }

  return value;
}

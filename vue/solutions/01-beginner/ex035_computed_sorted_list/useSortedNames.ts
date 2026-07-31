// Exercise 035 — useSortedNames composable (reference solution).
import { computed, ref, type ComputedRef, type Ref } from "vue";

export interface SortedNames {
  names: Ref<string[]>;
  sortedNames: ComputedRef<string[]>;
}

export function useSortedNames(initial: string[]): SortedNames {
  const names = ref(initial) as Ref<string[]>;
  const sortedNames = computed(() => [...names.value].sort((a, b) => a.localeCompare(b)));
  return { names, sortedNames };
}

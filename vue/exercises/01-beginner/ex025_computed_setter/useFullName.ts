// Exercise 025 — useFullName composable (beginner).
// Goal:   a writable computed with a custom get/set.
// Drills: computed(), splitting/joining strings, updating multiple refs from one setter.
import { computed, ref, type Ref, type WritableComputedRef } from "vue";

export interface FullName {
  firstName: Ref<string>;
  lastName: Ref<string>;
  fullName: WritableComputedRef<string>;
}

export function useFullName(initialFirst = "", initialLast = ""): FullName {
  const firstName = ref(initialFirst);
  const lastName = ref(initialLast);

  const fullName = computed<string>({
    get: () => {
      throw new Error("TODO: implement fullName getter");
    },
    set: (_value: string) => {
      throw new Error("TODO: implement fullName setter");
    },
  });

  return { firstName, lastName, fullName };
}

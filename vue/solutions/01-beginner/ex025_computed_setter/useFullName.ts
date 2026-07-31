// Exercise 025 — useFullName composable (reference solution).
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
    get: () => `${firstName.value} ${lastName.value}`,
    set: (value: string) => {
      const [first = "", ...rest] = value.trim().split(" ");
      firstName.value = first;
      lastName.value = rest.join(" ");
    },
  });

  return { firstName, lastName, fullName };
}

// Exercise 004 — useFullName composable (reference solution).
import { computed, ref, type Ref } from "vue";

export interface FullName {
  firstName: Ref<string>;
  lastName: Ref<string>;
  fullName: Ref<string>;
}

export function useFullName(first = "", last = ""): FullName {
  const firstName = ref(first);
  const lastName = ref(last);
  const fullName = computed(() => `${firstName.value} ${lastName.value}`);

  return {
    firstName,
    lastName,
    fullName: fullName as unknown as Ref<string>,
  };
}

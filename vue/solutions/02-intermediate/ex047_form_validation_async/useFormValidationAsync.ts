// Exercise 047 — useFormValidationAsync composable (reference solution).
import { ref, type Ref } from "vue";

export type CheckAvailable = (username: string) => Promise<boolean>;

export interface FormValidationAsync {
  username: Ref<string>;
  error: Ref<string | null>;
  isValidating: Ref<boolean>;
  validate: () => Promise<boolean>;
}

export function useFormValidationAsync(
  checkAvailable: CheckAvailable,
): FormValidationAsync {
  const username = ref("");
  const error = ref<string | null>(null);
  const isValidating = ref(false);

  async function validate(): Promise<boolean> {
    if (!username.value) {
      error.value = "Username is required";
      return false;
    }

    isValidating.value = true;
    try {
      const available = await checkAvailable(username.value);
      error.value = available ? null : "Username is already taken";
      return available;
    } finally {
      isValidating.value = false;
    }
  }

  return { username, error, isValidating, validate };
}

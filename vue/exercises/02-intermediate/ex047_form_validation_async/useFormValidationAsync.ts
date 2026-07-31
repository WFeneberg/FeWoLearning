// Exercise 047 — useFormValidationAsync composable (intermediate).
// Goal:   a form field whose validation calls an injected async
//         `checkAvailable(username)` function and sets an error when it
//         resolves false.
// Drills: async composables, race-condition-safe state updates, awaiting
//         reactive side effects in tests.
import { ref, type Ref } from "vue";

export type CheckAvailable = (username: string) => Promise<boolean>;

export interface FormValidationAsync {
  username: Ref<string>;
  error: Ref<string | null>;
  isValidating: Ref<boolean>;
  validate: () => Promise<boolean>;
}

export function useFormValidationAsync(
  _checkAvailable: CheckAvailable,
): FormValidationAsync {
  throw new Error("TODO: implement useFormValidationAsync");
}

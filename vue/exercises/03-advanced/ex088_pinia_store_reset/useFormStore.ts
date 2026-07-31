// Exercise 088 — Pinia store reset and $patch (advanced).
// Goal:   a Pinia-style form store whose `$patch` applies a partial update
//         to several state fields at once (in a single reactive mutation),
//         whose `$reset` restores the state to exactly the initial snapshot
//         captured when the store was created (not to hard-coded defaults),
//         and whose `resetForm` action is simply `$reset` exposed as a named
//         action, the way a real Pinia store's actions call `this.$reset()`.
// Drills: Pinia's `$patch`/`$reset` store-instance methods, bulk reactive
//         updates vs. one-field-at-a-time assignment, and capturing an
//         immutable "initial state" snapshot that survives later patches.
import { reactive } from "vue";

export interface FormState {
  firstName: string;
  lastName: string;
  email: string;
  subscribed: boolean;
}

export interface FormStore extends FormState {
  /** Applies a partial update to several fields at once. */
  $patch: (partial: Partial<FormState>) => void;
  /** Restores every field to the value captured when the store was created. */
  $reset: () => void;
  /** Named action that resets the form; implemented via `$reset`. */
  resetForm: () => void;
}

const defaultState: FormState = {
  firstName: "",
  lastName: "",
  email: "",
  subscribed: false,
};

/**
 * Creates a fresh form store. `initialOverrides` seeds the initial state
 * (and is what `$reset`/`resetForm` restore back to), merged over the
 * built-in defaults.
 */
export function useFormStore(
  _initialOverrides: Partial<FormState> = {},
): FormStore {
  throw new Error("TODO: implement useFormStore");
}

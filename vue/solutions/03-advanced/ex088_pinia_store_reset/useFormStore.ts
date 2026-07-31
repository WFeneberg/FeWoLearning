// Exercise 088 — Pinia store reset and $patch (reference solution).
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
  initialOverrides: Partial<FormState> = {},
): FormStore {
  // Captured once, up front, so later `$patch` calls can never disturb the
  // snapshot that `$reset` restores back to.
  const initialState: FormState = { ...defaultState, ...initialOverrides };

  const state = reactive<FormState>({ ...initialState });

  const store: FormStore = {
    get firstName() {
      return state.firstName;
    },
    get lastName() {
      return state.lastName;
    },
    get email() {
      return state.email;
    },
    get subscribed() {
      return state.subscribed;
    },
    $patch(partial) {
      Object.assign(state, partial);
    },
    $reset() {
      Object.assign(state, initialState);
    },
    resetForm() {
      store.$reset();
    },
  };

  return store;
}

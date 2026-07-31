// Exercise 063 — useValidation composable (reference solution).
import { computed, type ComputedRef, type Ref } from "vue";

/** A rule inspects the current value and returns an error message when it
 *  fails, or `null` when the value satisfies the rule. */
export type ValidationRule = (value: string) => string | null;

export interface Validation {
  errors: ComputedRef<string[]>;
  isValid: ComputedRef<boolean>;
}

export function useValidation(
  value: Ref<string>,
  rules: ValidationRule[],
): Validation {
  const errors = computed(() =>
    rules
      .map((rule) => rule(value.value))
      .filter((message): message is string => message !== null),
  );
  const isValid = computed(() => errors.value.length === 0);

  return { errors, isValid };
}

export const required: ValidationRule = (value) =>
  value.trim().length === 0 ? "This field is required." : null;

export const minLength = (min: number): ValidationRule => (value) =>
  value.length < min ? `Must be at least ${min} characters.` : null;

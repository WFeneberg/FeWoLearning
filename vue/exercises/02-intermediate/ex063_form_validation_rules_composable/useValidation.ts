// Exercise 063 — useValidation composable (intermediate).
// Goal:   apply an array of rule functions to a reactive form value and
//         expose the list of failing error messages as a computed.
// Drills: composable-based validation, computed derived from a Ref + rules,
//         designing a small rule-function contract.
import { type ComputedRef, type Ref } from "vue";

/** A rule inspects the current value and returns an error message when it
 *  fails, or `null` when the value satisfies the rule. */
export type ValidationRule = (value: string) => string | null;

export interface Validation {
  errors: ComputedRef<string[]>;
  isValid: ComputedRef<boolean>;
}

export function useValidation(
  _value: Ref<string>,
  _rules: ValidationRule[],
): Validation {
  throw new Error("TODO: implement useValidation");
}

export const required: ValidationRule = (_value) => {
  throw new Error("TODO: implement required");
};

export const minLength = (_min: number): ValidationRule => {
  throw new Error("TODO: implement minLength");
};

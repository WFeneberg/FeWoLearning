// Exercise 096 — Schema-driven form engine (expert).
// Goal:   turn a declarative field schema into a working reactive form: initial
//         values derived from each field's type, per-field validation driven by
//         the schema's rules, an aggregate validity flag, and a submit that
//         refuses to run while anything is invalid.
// Drills: reactive() over a derived shape, computed validation maps, mapping a
//         declarative spec onto reactive state, discriminated field types.
import { type ComputedRef } from "vue";

export type FieldType = "text" | "number" | "checkbox";

export interface FieldSchema {
  name: string;
  type: FieldType;
  label?: string;
  required?: boolean;
  /** Minimum length for text, minimum value for number. Ignored for checkbox. */
  min?: number;
  /** Maximum length for text, maximum value for number. Ignored for checkbox. */
  max?: number;
  /** Extra rule: return an error message, or null when the value is fine. */
  validate?: (value: unknown) => string | null;
}

export type FormValues = Record<string, string | number | boolean>;

export interface SchemaForm {
  /** Reactive current values, keyed by field name. */
  values: FormValues;
  /** One entry per invalid field: name → first error message. */
  errors: ComputedRef<Record<string, string>>;
  /** True when `errors` is empty. */
  isValid: ComputedRef<boolean>;
  /** Restores every field to its type's initial value. */
  reset: () => void;
  /**
   * Calls `onSubmit` with a plain snapshot of the values and returns true, but
   * only when the form is valid. Returns false and does not call it otherwise.
   */
  submit: (onSubmit: (values: FormValues) => void) => boolean;
}

/**
 * Builds a form from `schema`.
 *
 * Initial values by type: "text" → "", "number" → 0, "checkbox" → false.
 *
 * Validation order per field, first failure wins:
 *  1. `required` — empty string, or a false checkbox, is "is required";
 *  2. `min`/`max` — for text compare `value.length`, for number the value itself,
 *     reported as "must be at least N" / "must be at most N"
 *     (text: "must be at least N characters" / "must be at most N characters");
 *  3. the field's own `validate` callback.
 */
export function useSchemaForm(_schema: FieldSchema[]): SchemaForm {
  throw new Error("TODO: implement useSchemaForm");
}

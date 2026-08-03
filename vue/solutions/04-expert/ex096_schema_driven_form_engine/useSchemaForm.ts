// Exercise 096 — Schema-driven form engine (reference solution).
import { computed, reactive, type ComputedRef } from "vue";

export type FieldType = "text" | "number" | "checkbox";

export interface FieldSchema {
  name: string;
  type: FieldType;
  label?: string;
  required?: boolean;
  min?: number;
  max?: number;
  validate?: (value: unknown) => string | null;
}

export type FormValues = Record<string, string | number | boolean>;

export interface SchemaForm {
  values: FormValues;
  errors: ComputedRef<Record<string, string>>;
  isValid: ComputedRef<boolean>;
  reset: () => void;
  submit: (onSubmit: (values: FormValues) => void) => boolean;
}

function initialValue(type: FieldType): string | number | boolean {
  switch (type) {
    case "number":
      return 0;
    case "checkbox":
      return false;
    default:
      return "";
  }
}

/** First failing rule wins: required, then bounds, then the custom callback. */
function validateField(field: FieldSchema, value: unknown): string | null {
  if (field.required) {
    const missing = field.type === "checkbox" ? value !== true : String(value ?? "") === "";
    if (missing) return "is required";
  }

  if (field.type === "text") {
    const length = String(value ?? "").length;
    if (field.min !== undefined && length < field.min) {
      return `must be at least ${field.min} characters`;
    }
    if (field.max !== undefined && length > field.max) {
      return `must be at most ${field.max} characters`;
    }
  } else if (field.type === "number") {
    const num = Number(value ?? 0);
    if (field.min !== undefined && num < field.min) return `must be at least ${field.min}`;
    if (field.max !== undefined && num > field.max) return `must be at most ${field.max}`;
  }

  return field.validate ? field.validate(value) : null;
}

export function useSchemaForm(schema: FieldSchema[]): SchemaForm {
  const seed = (): FormValues =>
    Object.fromEntries(schema.map((f) => [f.name, initialValue(f.type)]));

  const values = reactive<FormValues>(seed());

  const errors = computed(() => {
    const result: Record<string, string> = {};
    for (const field of schema) {
      const message = validateField(field, values[field.name]);
      if (message !== null) result[field.name] = message;
    }
    return result;
  });

  const isValid = computed(() => Object.keys(errors.value).length === 0);

  const reset = (): void => {
    Object.assign(values, seed());
  };

  const submit = (onSubmit: (values: FormValues) => void): boolean => {
    if (!isValid.value) return false;
    // A plain copy, so the caller cannot be surprised by later edits.
    onSubmit({ ...values });
    return true;
  };

  return { values, errors, isValid, reset, submit };
}

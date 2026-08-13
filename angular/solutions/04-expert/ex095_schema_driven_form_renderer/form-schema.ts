import { FormBuilder, FormControl, FormGroup, ValidatorFn, Validators } from "@angular/forms";

// Exercise 095 — a schema-driven form renderer: schema -> reactive form (reference solution).

export type FieldType = "text" | "number" | "checkbox";

export interface FieldValidatorSpec {
  readonly required?: boolean;
  readonly minLength?: number;
  readonly min?: number;
  readonly max?: number;
}

export interface FieldSchema {
  readonly name: string;
  readonly type: FieldType;
  readonly defaultValue?: string | number | boolean;
  readonly validators?: FieldValidatorSpec;
}

function defaultValueFor(type: FieldType): string | number | boolean {
  switch (type) {
    case "text":
      return "";
    case "number":
      return 0;
    case "checkbox":
      return false;
  }
}

function validatorsFor(spec: FieldValidatorSpec | undefined): ValidatorFn[] {
  if (!spec) return [];

  const fns: ValidatorFn[] = [];
  if (spec.required) fns.push(Validators.required);
  if (spec.minLength !== undefined) fns.push(Validators.minLength(spec.minLength));
  if (spec.min !== undefined) fns.push(Validators.min(spec.min));
  if (spec.max !== undefined) fns.push(Validators.max(spec.max));
  return fns;
}

export function buildFormFromSchema(fb: FormBuilder, schema: readonly FieldSchema[]): FormGroup {
  const controls: Record<string, FormControl> = {};

  // `??` (not `||`) — an explicit false/0/"" defaultValue must survive, only null/undefined falls
  // back to the per-type default.
  for (const field of schema) {
    const initialValue = field.defaultValue ?? defaultValueFor(field.type);
    controls[field.name] = fb.control(initialValue, validatorsFor(field.validators));
  }

  return fb.group(controls);
}

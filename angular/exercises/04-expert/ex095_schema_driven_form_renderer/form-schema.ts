import { FormBuilder, FormGroup } from "@angular/forms";

// Exercise 095 — a schema-driven form renderer: schema -> reactive form (expert).
// Goal:   turn a plain data description of a form (name, type, default, validators) into a real
//         FormGroup, so adding a field to a form is a data change, not a template change.
// Drills: `FormBuilder.control`/`.group` driven entirely by a loop over data instead of hand-written
//         per-field calls, mapping a small validator-spec object to the actual `ValidatorFn`s from
//         `Validators`, and inferring per-type default values when the schema does not specify one.
// Passes: when `npx jest exercises/04-expert/ex095_schema_driven_form_renderer` is green.
//
// Exercise 039's SignupFormComponent hardcodes three named fields directly into `fb.group({...})` —
// fine when the form's shape is fixed at compile time, useless the moment a form's fields need to
// come from something the app does not control at build time (a CMS-configured contact form, a
// per-tenant settings screen, anything where "which fields exist" is data). `buildFormFromSchema`
// is the general version: given ANY `FieldSchema[]`, it builds the FormGroup that data describes —
// there is no schema-specific code inside this function at all.
//
// Two schemas fed through this same function must produce two fully independent FormGroups (the
// same isolation property exercise 093's store factory has to satisfy) — this is a pure function of
// its `schema` argument, never memoizing or reusing a control across calls.
//
// A field with no `defaultValue` still needs SOME initial value (a FormControl always has one), so
// an unspecified default is inferred from the field's `type` — `""` for text, `0` for number,
// `false` for checkbox — rather than `undefined`/`null`, which would make the control's value type
// unpredictable for whatever ends up rendering it. An EXPLICIT `defaultValue` of `false` or `0` is
// still a real value, not "missing" — do not fall back to the per-type default in that case.

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

/**
 * TODO: implement buildFormFromSchema.
 *   - One control per FieldSchema entry, keyed by `name`, built with `fb.control(...)`.
 *   - The control's initial value is `field.defaultValue` if one was given (including `false`/`0`/
 *     `""` — those are real values, not "missing"), otherwise the per-type default: `""` for
 *     "text", `0` for "number", `false` for "checkbox".
 *   - Translate `field.validators` into actual ValidatorFns: `required` -> `Validators.required`,
 *     `minLength` -> `Validators.minLength(n)`, `min` -> `Validators.min(n)`,
 *     `max` -> `Validators.max(n)`. A field with no `validators` (or an empty object) gets no
 *     validators at all.
 *   - Assemble all the controls into one `fb.group({...})` and return it.
 */
export function buildFormFromSchema(fb: FormBuilder, schema: readonly FieldSchema[]): FormGroup {
  throw new Error("TODO: implement buildFormFromSchema");
}

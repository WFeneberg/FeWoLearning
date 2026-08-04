import { Component } from "@angular/core";
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";

// Exercise 040 — built-in synchronous validators (reference solution).

/**
 * Message builders per error key, in display order.
 *
 * A list rather than a lookup, so the order does not depend on Object.keys, and an error key
 * with no entry here is simply skipped instead of rendering "[object Object]".
 */
const MESSAGES: ReadonlyArray<[string, (name: string, detail: unknown) => string]> = [
  ["required", (name) => `${name} is required`],
  ["email", (name) => `${name} must be an email address`],
  [
    "minlength",
    (name, detail) =>
      `${name} must be at least ${(detail as { requiredLength: number }).requiredLength} characters`,
  ],
  ["min", (name, detail) => `${name} must be at least ${(detail as { min: number }).min}`],
  ["max", (name, detail) => `${name} must be at most ${(detail as { max: number }).max}`],
  ["pattern", (name) => `${name} is not in the expected format`],
];

@Component({
  selector: "app-registration-form",
  standalone: true,
  imports: [ReactiveFormsModule],
  template: `
    <form [formGroup]="form">
      <input class="email" formControlName="email" />
      <input class="password" formControlName="password" />
      <input class="age" type="number" formControlName="age" />
      <input class="code" formControlName="code" />
    </form>
    <ul class="errors">
      @for (message of visibleErrors(); track message) {
        <li class="error">{{ message }}</li>
      }
    </ul>
    <button class="submit" type="button" [disabled]="form.invalid">Submit</button>
  `,
})
export class RegistrationFormComponent {
  readonly form = new FormGroup({
    email: new FormControl("", {
      nonNullable: true,
      validators: [Validators.required, Validators.email],
    }),
    password: new FormControl("", {
      nonNullable: true,
      // The factory is minLength; the error key it produces is lower-case minlength.
      validators: [Validators.required, Validators.minLength(8)],
    }),
    age: new FormControl(0, {
      nonNullable: true,
      validators: [Validators.min(18), Validators.max(120)],
    }),
    code: new FormControl("", {
      nonNullable: true,
      // Not required, and a pattern validator has nothing to say about an empty value.
      validators: [Validators.pattern(/^[A-Z]{3}-\d{4}$/)],
    }),
  });

  controlFor(name: string): FormControl {
    const control = this.form.get(name);
    if (control === null) {
      throw new Error(`no control named "${name}"`);
    }
    return control as FormControl;
  }

  messagesFor(name: string): readonly string[] {
    const errors = this.controlFor(name).errors;
    if (errors === null) {
      return [];
    }
    // Driven by MESSAGES rather than by Object.keys(errors), so the order is stable and an
    // unrecognised key is simply skipped.
    return MESSAGES.filter(([key]) => key in errors).map(([key, build]) =>
      build(name, errors[key]),
    );
  }

  visibleErrors(): readonly string[] {
    return Object.keys(this.form.controls).flatMap((name) => {
      const control = this.controlFor(name);
      // A fresh form is invalid but untouched; shouting at that point is the bug this avoids.
      return control.touched || control.dirty ? this.messagesFor(name) : [];
    });
  }

  revealAllErrors(): void {
    this.form.markAllAsTouched();
  }
}

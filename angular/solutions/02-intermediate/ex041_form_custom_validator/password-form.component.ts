import { Component } from "@angular/core";
import {
  AbstractControl,
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  ValidationErrors,
  ValidatorFn,
} from "@angular/forms";

// Exercise 041 — writing your own validators (reference solution).

export function forbiddenWords(words: readonly string[]): ValidatorFn {
  // A factory: the configuration is captured once and the returned function is what Angular
  // calls. This is exactly how Validators.minLength(8) is built.
  return (control: AbstractControl): ValidationErrors | null => {
    const value = String(control.value ?? "").toLowerCase();
    if (value === "") {
      return null;
    }
    // Scans `words` in order, so the reported word is predictable rather than incidental.
    const hit = words.find((word) => value.includes(word.toLowerCase()));
    // null means valid. Returning something truthy for success is the classic inversion bug.
    return hit === undefined ? null : { forbidden: { word: hit } };
  };
}

export function fieldsMatch(first: string, second: string): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    // `control` is the group, because a rule about two fields belongs to neither of them.
    const a = control.get(first)?.value ?? "";
    const b = control.get(second)?.value ?? "";
    if (a === "" || b === "") {
      // Nothing to compare yet — otherwise a fresh form reports a mismatch immediately.
      return null;
    }
    return a === b ? null : { mismatch: { first, second } };
  };
}

@Component({
  selector: "app-password-form",
  standalone: true,
  imports: [ReactiveFormsModule],
  template: `
    <form [formGroup]="form">
      <input class="username" formControlName="username" />
      <input class="password" formControlName="password" />
      <input class="confirm" formControlName="confirm" />
    </form>
    <p class="mismatch">{{ form.hasError("mismatch") ? "passwords must match" : "" }}</p>
  `,
})
export class PasswordFormComponent {
  readonly form = new FormGroup(
    {
      username: new FormControl("", {
        nonNullable: true,
        validators: [forbiddenWords(["admin", "root"])],
      }),
      password: new FormControl("", { nonNullable: true }),
      confirm: new FormControl("", { nonNullable: true }),
    },
    // Group-level, so the error lands on the group and not on either control.
    { validators: [fieldsMatch("password", "confirm")] },
  );

  controlFor(name: string): FormControl {
    const control = this.form.get(name);
    if (control === null) {
      throw new Error(`no control named "${name}"`);
    }
    return control as FormControl;
  }

  forbiddenWord(): string | null {
    const error = this.controlFor("username").getError("forbidden") as
      | { word: string }
      | undefined;
    return error?.word ?? null;
  }

  hasMismatch(): boolean {
    return this.form.hasError("mismatch");
  }

  reflectMismatchOntoConfirm(): void {
    const confirm = this.controlFor("confirm");
    // setErrors replaces the control's whole error object, so this is only safe because
    // `confirm` has no validators of its own to overwrite.
    confirm.setErrors(this.hasMismatch() ? { mismatch: true } : null);
  }
}

export type { AbstractControl, ValidationErrors };

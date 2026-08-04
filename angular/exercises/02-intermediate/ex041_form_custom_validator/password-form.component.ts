import { Component } from "@angular/core";
import { AbstractControl, FormControl, FormGroup, ValidationErrors, ValidatorFn } from "@angular/forms";

// Exercise 041 — writing your own validators (intermediate).
// Goal:   validate a single control, and then validate two controls against each other.
// Drills: the ValidatorFn contract, returning a useful error payload, a validator *factory*
//         parameterised at construction, and a group-level validator for a cross-field rule.
// Passes: when `npx jest exercises/02-intermediate/ex041_form_custom_validator` is green.
//
// The contract is one line: `(control) => ValidationErrors | null`, where null means valid.
// Getting that inverted — returning something truthy for success — makes everything valid-
// looking and is the single most common mistake here.
//
// Two shapes. A plain ValidatorFn is passed directly. A *factory* takes configuration and
// returns a ValidatorFn, which is how Validators.minLength(8) works and how you parameterise
// your own. Put whatever a message will need into the payload: `{forbidden: {word: "admin"}}`
// lets the UI name the problem instead of saying "invalid".
//
// The cross-field case is where people go wrong structurally. "These two fields must match" is
// not a property of either field, so it cannot live on either control — a validator on
// `confirm` cannot see `password`. It goes on the *group*, and the error lands on the group.
// That means `confirm.errors` stays null while the form is invalid, so the template has to look
// at `form.errors` for it. Copying the error down onto the control is a common convenience,
// and this exercise does it explicitly so the difference is visible.
//
// Template contract the spec asserts (classes are the query hooks — keep them):
//   <form [formGroup]="form">
//     <input class="username" formControlName="username" />
//     <input class="password" formControlName="password" />
//     <input class="confirm" formControlName="confirm" />
//   </form>
//   <p class="mismatch">{{ form.hasError("mismatch") ? "passwords must match" : "" }}</p>

/**
 * TODO: reject any value containing one of `words`, compared case-insensitively.
 *
 * Returns `{forbidden: {word: <the matched word>}}` naming the *first* word in `words` that
 * appears, or null when the value is clean. An empty value has nothing to reject.
 */
export function forbiddenWords(words: readonly string[]): ValidatorFn {
  throw new Error("TODO: implement forbiddenWords");
}

/**
 * TODO: a group validator requiring two named controls to hold the same value.
 *
 * Returns `{mismatch: {first, second}}` with the two control names, or null when they agree.
 * While either control is empty there is nothing to compare yet, so return null — otherwise a
 * fresh form reports a mismatch before the user has typed anything.
 *
 * The `control` handed in is the group, so reach the children with control.get(name).
 */
export function fieldsMatch(first: string, second: string): ValidatorFn {
  throw new Error("TODO: implement fieldsMatch");
}

@Component({
  selector: "app-password-form",
  standalone: true,
  // TODO: import ReactiveFormsModule.
  template: `<p>TODO: render the form — see the template contract above</p>`,
})
export class PasswordFormComponent {
  /**
   * TODO: a FormGroup of three non-nullable string controls — username, password, confirm.
   *
   * `username` carries forbiddenWords(["admin", "root"]). The group itself carries
   * fieldsMatch("password", "confirm").
   *
   * Declared empty so the stub compiles — replace the declaration.
   */
  readonly form = new FormGroup({});

  /** One control by name. */
  controlFor(name: string): FormControl {
    throw new Error("TODO: implement controlFor");
  }

  /** The forbidden word the username tripped over, or null. */
  forbiddenWord(): string | null {
    throw new Error("TODO: implement forbiddenWord");
  }

  /** Whether the two password fields disagree. */
  hasMismatch(): boolean {
    throw new Error("TODO: implement hasMismatch");
  }

  /**
   * Copy the group's mismatch error down onto the `confirm` control, or clear it.
   *
   * A convenience for templates that render errors per field. Note it must not clobber
   * `confirm`'s own errors — here it has none, which is why this is safe.
   */
  reflectMismatchOntoConfirm(): void {
    throw new Error("TODO: implement reflectMismatchOntoConfirm");
  }
}

/** Re-exported so the spec can build controls to test the validators in isolation. */
export type { AbstractControl, ValidationErrors };

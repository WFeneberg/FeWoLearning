import { Component } from "@angular/core";
import { FormControl, FormGroup } from "@angular/forms";

// Exercise 040 — built-in synchronous validators (intermediate).
// Goal:   attach validation rules to controls and turn their output into something a user
//         could actually read.
// Drills: Validators.required / email / minLength / min / max / pattern, the shape of
//         `control.errors`, hasError, and surfacing errors only once a field has been touched.
// Passes: when `npx jest exercises/02-intermediate/ex040_form_sync_validators` is green.
//
// A validator returns `null` for valid and an *object* for invalid, and that object is the
// error payload: `Validators.minLength(8)` produces `{minlength: {requiredLength: 8,
// actualLength: 3}}`. Two things to note. The key is lower-case `minlength` while the factory
// is camel-case `minLength`, which is a classic half-hour of confusion. And the payload carries
// data, so a message can say "needs 8, got 3" rather than "too short".
//
// The interaction rule matters as much as the rules themselves. A pristine, untouched form is
// invalid from the moment it is created — every required field is empty — so rendering errors
// straight from `invalid` shouts at the user before they have typed anything. Gate the display
// on `touched || dirty`; keep `valid` for whether submit is allowed.
//
// Template contract the spec asserts (classes are the query hooks — keep them):
//   <form [formGroup]="form">
//     <input class="email" formControlName="email" />
//     <input class="password" formControlName="password" />
//     <input class="age" type="number" formControlName="age" />
//     <input class="code" formControlName="code" />
//   </form>
//   <ul class="errors">
//     @for (message of visibleErrors(); track message) {
//       <li class="error">{{ message }}</li>
//     }
//   </ul>
//   <button class="submit" type="button" [disabled]="form.invalid">Submit</button>

@Component({
  selector: "app-registration-form",
  standalone: true,
  // TODO: import ReactiveFormsModule.
  template: `<p>TODO: render the form — see the template contract above</p>`,
})
export class RegistrationFormComponent {
  /**
   * TODO: a FormGroup of non-nullable controls with these validators:
   *
   *   email    "" — required, and a valid email address
   *   password "" — required, at least 8 characters
   *   age      0  — at least 18, at most 120
   *   code     "" — matches /^[A-Z]{3}-\d{4}$/ (not required, so "" must stay valid)
   *
   * Declared empty so the stub compiles — replace the declaration.
   */
  readonly form = new FormGroup({});

  /** One control by name. */
  controlFor(name: string): FormControl {
    throw new Error("TODO: implement controlFor");
  }

  /**
   * Readable messages for one control, in this order when several apply:
   *
   *   required   -> "<name> is required"
   *   email      -> "<name> must be an email address"
   *   minlength  -> "<name> must be at least <n> characters"
   *   min        -> "<name> must be at least <n>"
   *   max        -> "<name> must be at most <n>"
   *   pattern    -> "<name> is not in the expected format"
   *
   * An error key with no message of its own is ignored. A valid control gives [].
   */
  messagesFor(name: string): readonly string[] {
    throw new Error("TODO: implement messagesFor");
  }

  /**
   * Every message for every control, but only for controls the user has touched or edited.
   *
   * This is what the template renders, and it is why a fresh form shows nothing.
   */
  visibleErrors(): readonly string[] {
    throw new Error("TODO: implement visibleErrors");
  }

  /** Mark every control touched, as a submit attempt should, so all errors become visible. */
  revealAllErrors(): void {
    throw new Error("TODO: implement revealAllErrors");
  }
}

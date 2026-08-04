import { Component } from "@angular/core";
import { FormControl, FormGroup } from "@angular/forms";

// Exercise 038 — nested FormGroups (intermediate).
// Goal:   compose controls into a tree that mirrors the shape of your data.
// Drills: FormGroup, a group inside a group, setValue vs patchValue, get("a.b") paths, and
//         the difference between `value` and `getRawValue()` when something is disabled.
// Passes: when `npx jest exercises/02-intermediate/ex038_form_group_nested` is green.
//
// A FormGroup is a control that holds other controls, so `form.value` comes back shaped like
// the group — nested objects included. That is the appeal: the form tree matches the DTO and
// there is no assembling by hand.
//
// setValue and patchValue are not interchangeable. setValue is strict: every key must be
// present, and a missing one is an error, which is what you want when replacing a whole
// record. patchValue is lenient: it applies what it recognises and ignores the rest, which is
// what you want for a partial update. Reaching for patchValue by default means a typo in a
// key name fails silently.
//
// And the one that causes real bugs: a disabled control is *left out of* `form.value`
// entirely — not null, absent. If a submit handler reads `value`, disabling a field silently
// drops it from the payload. `getRawValue()` includes disabled controls, which is usually what
// a save actually wants.
//
// Template contract the spec asserts (classes are the query hooks — keep them):
//   <form [formGroup]="form">
//     <input class="name" formControlName="name" />
//     <div formGroupName="address">
//       <input class="street" formControlName="street" />
//       <input class="city" formControlName="city" />
//       <input class="zip" formControlName="zip" />
//     </div>
//   </form>
//   <p class="summary">{{ summary() }}</p>

@Component({
  selector: "app-address-form",
  standalone: true,
  // TODO: import ReactiveFormsModule.
  template: `<p>TODO: render the form — see the template contract above</p>`,
})
export class AddressFormComponent {
  /**
   * TODO: a FormGroup with a `name` control and a nested `address` group holding
   * `street`, `city` and `zip`. Every control is a non-nullable string starting at "".
   *
   * Declared empty so the stub compiles — replace the declaration.
   */
  readonly form = new FormGroup({});

  /** The nested address group, reached by name. */
  addressGroup(): FormGroup {
    throw new Error("TODO: implement addressGroup");
  }

  /** One nested control, reached with a dotted path such as "address.city". */
  controlAt(path: string): FormControl {
    throw new Error("TODO: implement controlAt");
  }

  /** `"<name>, <street>, <zip> <city>"`, with any blank part left out and no double commas. */
  summary(): string {
    throw new Error("TODO: implement summary");
  }

  /** Replace the whole form. Every key must be supplied — that is setValue's contract. */
  replaceAll(value: {
    name: string;
    address: { street: string; city: string; zip: string };
  }): void {
    throw new Error("TODO: implement replaceAll");
  }

  /** Apply a partial update, at any depth, leaving everything else alone. */
  applyPatch(changes: {
    name?: string;
    address?: { street?: string; city?: string; zip?: string };
  }): void {
    throw new Error("TODO: implement applyPatch");
  }

  /** The payload a save should send — disabled controls included. */
  payload(): unknown {
    throw new Error("TODO: implement payload");
  }
}

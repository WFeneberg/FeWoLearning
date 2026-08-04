import { Component } from "@angular/core";
import { FormControl, FormGroup } from "@angular/forms";

// Exercise 044 — the interaction flags (intermediate).
// Goal:   tell "the user has not tried yet" apart from "the user got it wrong".
// Drills: pristine/dirty and untouched/touched, which operations set them, markAsDirty /
//         markAsTouched / markAsPristine / markAllAsTouched, unsaved-change detection, and
//         gating both error display and submit on the right flag.
// Passes: when `npx jest exercises/02-intermediate/ex044_form_status_flags` is green.
//
// Validity alone cannot drive a form's UI. A freshly built form with required fields is invalid
// from birth, so `invalid` says nothing about whether the user has done anything wrong yet. The
// flags are what carry that:
//
//   dirty    — the *value* has been changed by the user
//   touched  — the control has been focused and blurred
//
// Both matter because they catch different users: someone typing and leaving a field half-filled
// is dirty, someone tabbing through without typing is touched.
//
// The rule that surprises people: a programmatic setValue does **not** dirty a control. Only user
// interaction — or an explicit markAsDirty — does. That is deliberate, and it is what makes dirty
// usable as "are there unsaved changes?": loading a record into the form with patchValue leaves
// it pristine, exactly as it should.
//
// markAsPristine is therefore what a successful save calls. Nothing else resets it, and a form
// that stays dirty after saving will nag about unsaved changes forever.
//
// Template contract the spec asserts (classes are the query hooks — keep them):
//   <form [formGroup]="form">
//     <input class="name" formControlName="name" />
//     <input class="email" formControlName="email" />
//   </form>
//   <p class="state">{{ stateLabel() }}</p>
//   <button class="save" type="button" [disabled]="!canSave()">Save</button>

@Component({
  selector: "app-profile-editor",
  standalone: true,
  // TODO: import ReactiveFormsModule.
  template: `<p>TODO: render the editor — see the template contract above</p>`,
})
export class ProfileEditorComponent {
  /**
   * TODO: a FormGroup of two non-nullable controls, both required:
   *
   *   name  ""
   *   email ""
   *
   * Declared empty so the stub compiles — replace the declaration.
   */
  readonly form = new FormGroup({});

  /** How many times save() has succeeded. */
  saveCount = 0;

  /** One control by name. */
  controlFor(name: string): FormControl {
    throw new Error("TODO: implement controlFor");
  }

  /**
   * Load an existing record into the form.
   *
   * Uses patchValue, so the form stays pristine — a loaded record is not an unsaved change.
   */
  load(record: { name: string; email: string }): void {
    throw new Error("TODO: implement load");
  }

  /** Whether a control's errors should be shown yet: only once it is touched or dirty. */
  showErrorsFor(name: string): boolean {
    throw new Error("TODO: implement showErrorsFor");
  }

  /** Whether there is anything worth saving: valid, and actually changed. */
  canSave(): boolean {
    throw new Error("TODO: implement canSave");
  }

  /** True when the user has edits that have not been saved. */
  hasUnsavedChanges(): boolean {
    throw new Error("TODO: implement hasUnsavedChanges");
  }

  /**
   * "clean" when pristine, "editing" when dirty but not yet valid, "ready" when both dirty and
   * valid — the three states the button and the label between them describe.
   */
  stateLabel(): string {
    throw new Error("TODO: implement stateLabel");
  }

  /**
   * Save.
   *
   * Refuses unless canSave(). On success, counts the save and marks the form pristine so it
   * stops reporting unsaved changes.
   */
  save(): void {
    throw new Error("TODO: implement save");
  }

  /** A failed submit attempt: reveal every error by marking the whole form touched. */
  revealErrors(): void {
    throw new Error("TODO: implement revealErrors");
  }
}

import { Component } from "@angular/core";
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";

// Exercise 044 — the interaction flags (reference solution).
@Component({
  selector: "app-profile-editor",
  standalone: true,
  imports: [ReactiveFormsModule],
  template: `
    <form [formGroup]="form">
      <input class="name" formControlName="name" />
      <input class="email" formControlName="email" />
    </form>
    <p class="state">{{ stateLabel() }}</p>
    <button class="save" type="button" [disabled]="!canSave()">Save</button>
  `,
})
export class ProfileEditorComponent {
  readonly form = new FormGroup({
    name: new FormControl("", { nonNullable: true, validators: [Validators.required] }),
    email: new FormControl("", { nonNullable: true, validators: [Validators.required] }),
  });

  saveCount = 0;

  controlFor(name: string): FormControl {
    const control = this.form.get(name);
    if (control === null) {
      throw new Error(`no control named "${name}"`);
    }
    return control as FormControl;
  }

  load(record: { name: string; email: string }): void {
    // patchValue does not dirty anything, which is exactly right: loading is not editing.
    this.form.patchValue(record);
  }

  showErrorsFor(name: string): boolean {
    const control = this.controlFor(name);
    // Two flags, two kinds of user: one who typed and left it wrong (dirty), and one who
    // tabbed straight through (touched).
    return control.invalid && (control.touched || control.dirty);
  }

  canSave(): boolean {
    return this.form.valid && this.form.dirty;
  }

  hasUnsavedChanges(): boolean {
    return this.form.dirty;
  }

  stateLabel(): string {
    if (this.form.pristine) {
      return "clean";
    }
    return this.form.valid ? "ready" : "editing";
  }

  save(): void {
    if (!this.canSave()) {
      return;
    }
    this.saveCount += 1;
    // The only thing that clears dirty. Without it the form reports unsaved changes forever.
    this.form.markAsPristine();
  }

  revealErrors(): void {
    // Touches every control without dirtying any — the form has not been edited, only rejected.
    this.form.markAllAsTouched();
  }
}

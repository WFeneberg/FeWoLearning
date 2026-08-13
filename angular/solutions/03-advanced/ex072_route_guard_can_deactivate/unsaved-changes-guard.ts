import { Component, Injectable, inject, signal } from "@angular/core";
import { CanDeactivateFn } from "@angular/router";

// Exercise 072 — a functional CanDeactivate guard (reference solution).

export interface CanComponentDeactivate {
  canDeactivate(): boolean;
}

@Injectable({ providedIn: "root" })
export class DiscardConfirmation {
  confirm(message: string): boolean {
    return window.confirm(message);
  }
}

@Component({
  selector: "app-note-editor",
  standalone: true,
  template: `
    <textarea class="note" [value]="text()" (input)="onInput($event)"></textarea>
    <button class="save" type="button" (click)="save()">Save</button>
  `,
})
export class NoteEditorComponent implements CanComponentDeactivate {
  private savedText = "";

  readonly text = signal("");

  onInput(event: Event): void {
    this.text.set((event.target as HTMLTextAreaElement).value);
  }

  isDirty(): boolean {
    return this.text() !== this.savedText;
  }

  save(): void {
    this.savedText = this.text();
  }

  canDeactivate(): boolean {
    return !this.isDirty();
  }
}

export const unsavedChangesGuard: CanDeactivateFn<CanComponentDeactivate> = (component) => {
  if (component.canDeactivate()) {
    return true;
  }

  // Only reached when there is something to lose — a clean component never triggers this.
  const confirmation = inject(DiscardConfirmation);
  return confirmation.confirm("You have unsaved changes. Leave anyway?");
};

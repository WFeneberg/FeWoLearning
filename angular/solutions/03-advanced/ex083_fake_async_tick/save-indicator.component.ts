import { Component, signal } from "@angular/core";

// Exercise 083 — fakeAsync / tick / flushMicrotasks (reference solution).

@Component({
  selector: "app-save-indicator",
  standalone: true,
  template: `
    <button type="button" class="save" (click)="save()">Save</button>
    <p class="state">{{ state() }}</p>
  `,
})
export class SaveIndicatorComponent {
  readonly state = signal<"idle" | "saving" | "saved">("idle");

  readonly validated = signal(false);

  save(): void {
    this.state.set("saving");
    this.validated.set(false);

    Promise.resolve().then(() => this.validated.set(true));

    setTimeout(() => this.state.set("saved"), 2000);
  }
}

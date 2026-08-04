import { Component, inject, Injectable, signal } from "@angular/core";

// Exercise 020 — component-level providers (reference solution).

// No providedIn: this service exists only where a component asks for it, which keeps the
// scoping decision at the use site instead of buried in the service.
@Injectable()
export class DraftStore {
  private readonly text = signal("");

  static instances = 0;

  constructor() {
    DraftStore.instances += 1;
  }

  value(): string {
    return this.text();
  }

  write(next: string): void {
    this.text.set(next);
  }

  isDirty(): boolean {
    return this.text().trim() !== "";
  }
}

@Injectable({ providedIn: "root" })
export class SaveCounter {
  count = 0;

  record(): void {
    this.count += 1;
  }
}

@Component({
  selector: "app-draft-panel",
  standalone: true,
  // One DraftStore per DraftPanelComponent instance, destroyed with it.
  providers: [DraftStore],
  template: `
    <p class="draft">{{ draft.value() }}</p>
    <p class="dirty">{{ draft.isDirty() ? "dirty" : "clean" }}</p>
    <button class="save" type="button" (click)="save()">Save</button>
  `,
})
export class DraftPanelComponent {
  readonly draft = inject(DraftStore);
  readonly saves = inject(SaveCounter);

  save(): void {
    // Shared counter goes up for everyone; the draft cleared is only this panel's.
    this.saves.record();
    this.draft.write("");
  }
}

@Component({
  selector: "app-shell",
  standalone: true,
  imports: [DraftPanelComponent],
  template: `
    <app-draft-panel />
    <app-draft-panel />
  `,
})
export class ShellComponent {}

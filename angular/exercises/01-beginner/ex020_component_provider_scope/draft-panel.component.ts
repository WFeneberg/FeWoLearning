import { Component, inject, Injectable, signal } from "@angular/core";

// Exercise 020 — component-level providers (beginner).
// Goal:   give each component instance its own copy of a service.
// Drills: a component's `providers` array, per-instance scope, contrasting it with a
//         root singleton, and a service whose lifetime matches its component.
// Passes: when `npx jest exercises/01-beginner/ex020_component_provider_scope` is green.
//
// Injectors form a tree. A component listed in `providers: [DraftStore]` gets its own
// injector node, so it and its children resolve DraftStore to an instance nobody else
// sees — and that instance is destroyed with the component. That is exactly right for
// per-screen scratch state like an unsaved draft, and exactly wrong for a shared cart.
//
// The other half of the lesson is the choice itself: `providedIn: "root"` means "one for
// the application", a component provider means "one per component instance". Getting it
// backwards is how two open editors end up overwriting each other's draft.

/**
 * Scratch state for one editor.
 *
 * Note there is no `providedIn` — this service is only available where a component
 * explicitly provides it, which is what keeps the scope decision at the use site.
 */
@Injectable()
export class DraftStore {
  private readonly text = signal("");

  /** How many DraftStore instances have ever been constructed, across the whole run. */
  static instances = 0;

  constructor() {
    DraftStore.instances += 1;
  }

  /** The current draft. */
  value(): string {
    throw new Error("TODO: implement value");
  }

  /** Replace the draft. */
  write(next: string): void {
    throw new Error("TODO: implement write");
  }

  /** True when there is anything worth saving (ignoring surrounding whitespace). */
  isDirty(): boolean {
    throw new Error("TODO: implement isDirty");
  }
}

/** Shared across the application, for contrast with the per-component DraftStore. */
@Injectable({ providedIn: "root" })
export class SaveCounter {
  count = 0;

  record(): void {
    this.count += 1;
  }
}

// Template contract the spec asserts (classes are the query hooks — keep them):
//   <p class="draft">{{ draft.value() }}</p>
//   <p class="dirty">{{ draft.isDirty() ? "dirty" : "clean" }}</p>
//   <button class="save" type="button" (click)="save()">Save</button>
@Component({
  selector: "app-draft-panel",
  standalone: true,
  // TODO: provide DraftStore here, so each panel instance gets one of its own.
  providers: [],
  template: `<p>TODO: render the panel — see the template contract above</p>`,
})
export class DraftPanelComponent {
  readonly draft = inject(DraftStore);
  readonly saves = inject(SaveCounter);

  /** Record a save on the shared counter, then clear this panel's own draft. */
  save(): void {
    throw new Error("TODO: implement save");
  }
}

/** Two panels side by side, which is what makes the scoping visible. */
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

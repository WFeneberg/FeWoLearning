import { Component, input, signal } from "@angular/core";

// Exercise 081 — @defer: triggers, placeholder and loading blocks (reference solution).

@Component({
  selector: "app-heavy-panel",
  standalone: true,
  template: `<p class="heavy">Heavy panel for {{ label() }}</p>`,
})
export class HeavyPanelComponent {
  readonly label = input.required<string>();
}

@Component({
  selector: "app-details-panel",
  standalone: true,
  imports: [HeavyPanelComponent],
  template: `
    <button type="button" class="reveal" (click)="reveal()">Show details</button>

    @defer (when shouldLoad(); prefetch on idle) {
      <app-heavy-panel class="panel" [label]="label()" />
    } @placeholder (minimum 0ms) {
      <p class="placeholder">Details hidden — click to load.</p>
    } @loading (minimum 0ms) {
      <p class="loading">Loading details…</p>
    } @error {
      <p class="error">Couldn't load details.</p>
    }
  `,
})
export class DetailsPanelComponent {
  protected readonly label = signal("Room 204");

  readonly shouldLoad = signal(false);

  reveal(): void {
    this.shouldLoad.set(true);
  }
}

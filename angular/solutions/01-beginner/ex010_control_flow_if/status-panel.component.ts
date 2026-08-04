import { Component, signal } from "@angular/core";

// Exercise 010 — StatusPanelComponent (reference solution).

export type Status = "loading" | "error" | "ready";

export interface Profile {
  readonly name: string;
  readonly email: string;
}

@Component({
  selector: "app-status-panel",
  standalone: true,
  // @if is part of the template language — no CommonModule, no NgIf import.
  template: `
    @if (status() === "loading") {
      <p class="loading">Loading…</p>
    } @else if (status() === "error") {
      <p class="error">{{ message() }}</p>
    } @else if (isEmpty()) {
      <p class="empty">Nothing here</p>
    } @else {
      <p class="ready">{{ count() }} items</p>
    }

    <!-- "as p" checks the value and binds the non-null result for the whole block. -->
    @if (profile(); as p) {
      <p class="profile">{{ p.name }} ({{ p.email }})</p>
    } @else {
      <p class="anonymous">Signed out</p>
    }
  `,
})
export class StatusPanelComponent {
  readonly status = signal<Status>("loading");
  readonly message = signal("");
  readonly count = signal(0);
  readonly profile = signal<Profile | null>(null);

  isEmpty(): boolean {
    // Empty is a *ready* state with nothing in it — mid-load there is nothing to say yet.
    return this.status() === "ready" && this.count() === 0;
  }
}

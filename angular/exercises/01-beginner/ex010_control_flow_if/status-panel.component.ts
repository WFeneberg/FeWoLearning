import { Component, signal } from "@angular/core";

// Exercise 010 — StatusPanelComponent (beginner).
// Goal:   render one branch of four with the built-in @if control flow.
// Drills: @if / @else if / @else, the `as` alias for binding a checked value, and the
//         fact that @if *removes* the other branches from the DOM rather than hiding them.
// Passes: when `npx jest exercises/01-beginner/ex010_control_flow_if` is green.
//
// @if is built into the template language, so there is nothing to import — no
// CommonModule, no NgIf. The branch that does not match is not in the DOM at all, which
// is why the spec asserts querySelector returns null rather than checking a CSS class.
//
// The `as` alias earns its keep with a nullable value: `@if (profile(); as p)` both
// checks it and gives the block a non-null binding, so the template never repeats the
// call or fights the type checker over `profile()!.name`.
//
// Template contract the spec asserts (classes are the query hooks — keep them):
//   @if (status() === "loading") {
//     <p class="loading">Loading…</p>
//   } @else if (status() === "error") {
//     <p class="error">{{ message() }}</p>
//   } @else if (isEmpty()) {
//     <p class="empty">Nothing here</p>
//   } @else {
//     <p class="ready">{{ count() }} items</p>
//   }
//
//   @if (profile(); as p) {
//     <p class="profile">{{ p.name }} ({{ p.email }})</p>
//   } @else {
//     <p class="anonymous">Signed out</p>
//   }

export type Status = "loading" | "error" | "ready";

export interface Profile {
  readonly name: string;
  readonly email: string;
}

@Component({
  selector: "app-status-panel",
  standalone: true,
  template: `<p>TODO: render the panel — see the template contract above</p>`,
})
export class StatusPanelComponent {
  readonly status = signal<Status>("loading");
  readonly message = signal("");
  readonly count = signal(0);
  readonly profile = signal<Profile | null>(null);

  /** True when the panel is ready but has nothing to show. */
  isEmpty(): boolean {
    throw new Error("TODO: implement isEmpty");
  }
}

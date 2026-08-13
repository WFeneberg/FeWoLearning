import { Component, input, signal } from "@angular/core";

// Exercise 081 — @defer: triggers, placeholder and loading blocks (advanced).
// Goal:   stop paying the bundle-size and render cost of a heavy child component until something
//         actually asks to see it.
// Drills: `@defer` / `@placeholder` / `@loading` / `@error`, and a `when` trigger driven by
//         application state instead of a viewport/idle heuristic that jsdom cannot observe.
// Passes: when `npx jest exercises/03-advanced/ex081_defer_block` is green.
//
// `@defer` splits a piece of template into its own lazily-loaded chunk: the component(s) referenced
// inside the block are not even in the initial bundle, and nothing about them is instantiated until
// the trigger condition says otherwise. That is a stronger guarantee than `@if` — an `@if`'s branch
// still has to be compiled and shipped up front even while it is hidden; `@defer`'s branch does not
// exist on the page (or on the wire) until triggered at all.
//
// A trigger is just the condition that flips the block from "not yet" to "load now." `on viewport`
// / `on idle` / `on interaction` all describe real-world signals a browser can observe, none of
// which jsdom has any concept of — there is no scroll position, no idle callback, no hover in a
// headless DOM. `when <expression>` sidesteps that entirely: the expression can be *any* boolean —
// here, a plain signal this component controls itself — so the trigger is exercised the same way in
// a test as it would be by a real click in a browser.
//
// The four sub-blocks describe every state the deferred content can be in: `@placeholder` before
// loading starts (optionally after `minimum` so a placeholder does not flicker on a fast network),
// `@loading` while the chunk is being fetched, the default block once it has arrived, and `@error`
// if the fetch failed. Angular's `TestBed` intercepts all of this in tests by default (see the
// spec) — trigger conditions are not evaluated at all; a `DeferBlockFixture` is used to render
// each named state on demand, deterministically, regardless of what the real trigger would do.
//
// Template contract the spec asserts (classes are the query hooks — keep the nesting):
//   <button class="reveal" type="button" (click)="reveal()">Show details</button>
//   @defer (when shouldLoad(); prefetch on idle) {
//     <app-heavy-panel class="panel" [label]="label()" />
//   } @placeholder (minimum 0ms) {
//     <p class="placeholder">Details hidden — click to load.</p>
//   } @loading (minimum 0ms) {
//     <p class="loading">Loading details…</p>
//   } @error {
//     <p class="error">Couldn't load details.</p>
//   }

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
  template: `<p>TODO: implement the defer block — see the template contract in the header comment</p>`,
})
export class DetailsPanelComponent {
  protected readonly label = signal("Room 204");

  /** The @defer block's `when` trigger. Public so this component's own tests can read it. */
  readonly shouldLoad = signal(false);

  /**
   * TODO: implement reveal — flip `shouldLoad` so the @defer block's `when` condition becomes
   * true. The button that calls this is the only thing in this exercise that triggers loading;
   * nothing about that is automatic.
   */
  reveal(): void {
    throw new Error("TODO: implement reveal");
  }
}

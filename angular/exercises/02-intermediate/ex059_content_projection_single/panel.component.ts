import { Component } from "@angular/core";

// Exercise 059 — single-slot content projection (intermediate).
// Goal:   let a component's caller supply the content it wraps.
// Drills: <ng-content>, fallback content for when nothing is projected, the fact that projected
//         content belongs to the *parent* rather than the child, and where its bindings resolve.
// Passes: when `npx jest exercises/02-intermediate/ex059_content_projection_single` is green.
//
// A component that hard-codes its inner markup can only ever be used one way. `<ng-content>` moves
// that decision to the caller: the panel owns the frame, the caller owns the contents.
//
// The rule that trips people up is whose scope the projected markup lives in. Content written
// between `<app-panel>` and `</app-panel>` is compiled as part of the *parent's* template, so its
// bindings resolve against the parent, and the panel cannot see them. It is not "moved into" the
// child — it is rendered by the parent and merely placed inside the child's DOM.
//
// A consequence worth knowing: the projected content is instantiated whether or not an
// <ng-content> renders it. Wrapping <ng-content> in a false @if hides it but does not prevent its
// creation, so a heavy projected component still pays its construction cost.
//
// Angular 18 lets <ng-content> carry fallback content, used when the caller projects nothing.
//
// Template contract the spec asserts (classes are the query hooks — keep them):
//   <section class="panel">
//     <h3 class="heading">{{ heading }}</h3>
//     <div class="body">
//       <ng-content>nothing here yet</ng-content>
//     </div>
//   </section>

@Component({
  selector: "app-panel",
  standalone: true,
  template: `<p>TODO: render the panel — see the template contract above</p>`,
})
export class PanelComponent {
  /** TODO: an input for the heading, defaulting to "Panel". */
  heading = "Panel";
}

import { Component, input } from "@angular/core";

// Exercise 090 — @for track correctness and DOM reuse (advanced).
// Goal:   pick a `track` expression that lets Angular reuse existing DOM nodes across re-renders,
//         instead of silently destroying and recreating them.
// Passes: when `npx jest exercises/03-advanced/ex090_performance_track_by` is green.
//
// `@for` requires a `track` expression precisely so it never has to guess: given a new array, it
// diffs the *tracked keys* against the previous render's keys to decide, per item, "reuse this DOM
// node" versus "this is new, create one" versus "this one is gone, destroy it." Two ways to get that
// wrong, both innocent-looking:
//
//   track $index  — ties DOM identity to *position*, not to the item. Reorder the array (drag to
//   reorder, sort, unshift) and the item that used to be at index 3 is now at index 0 — but Angular,
//   tracking only positions, thinks "index 0 is still index 0" and reuses that DOM node for whatever
//   item now sits there. Any state the DOM itself was holding (an uncontrolled `<input>`'s typed
//   text, scroll position, a CSS transition mid-flight, focus) now visibly belongs to the wrong row.
//
//   track item  (the object reference itself) — fixes the reorder case, but breaks the far more
//   common one: a fresh array of fresh objects from a refetch, each logically the *same* entity as
//   before (same id) but a *new* object instance (immutable-update discipline, or just a new HTTP
//   response). Tracking by reference sees 100% new keys and destroys and recreates every row, even
//   though nothing the user cares about changed — exactly the wasted work `@for`'s tracking exists
//   to avoid, and, same as above, it loses whatever the destroyed DOM nodes were holding.
//
// `track item.id` survives both: a stable primitive key that follows the entity regardless of which
// index it now sits at or which object instance currently represents it.
//
// This component's `<input class="local-input">` per row has no binding to any model at all — it is
// deliberately "dumb" DOM state with nothing Angular tracks. That is the point: it exists only so a
// test can type into it, re-render the list, and check whether the *same* `<input>` element (and
// therefore its typed value) survived, which is the only reliable, observable proxy for "did Angular
// reuse this DOM node or throw it away and build a new one."

export interface TaggedItem {
  readonly id: number;
  readonly label: string;
}

@Component({
  selector: "app-tagged-list",
  standalone: true,
  template: `
    <ul>
      @for (item of items(); track trackItem(item)) {
        <li [attr.data-id]="item.id">
          {{ item.label }}
          <input class="local-input" />
        </li>
      }
    </ul>
  `,
})
export class TaggedListComponent {
  readonly items = input.required<readonly TaggedItem[]>();

  /**
   * TODO: implement trackItem — return the stable key `@for` should track this item by. It must
   * follow the same logical entity across both a reorder of the existing array (same object
   * references, new positions) and a full replacement with brand-new object instances that share the
   * same ids.
   */
  trackItem(item: TaggedItem): number {
    throw new Error("TODO: implement trackItem");
  }
}

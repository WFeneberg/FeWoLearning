import { Component, output, signal } from "@angular/core";

// Exercise 009 — SearchBoxComponent (beginner).
// Goal:   the same child-to-parent notifications as exercise 008, the modern way.
// Drills: output(), emit() with a typed payload, output<void>() for a bare signal,
//         subscribing to an OutputEmitterRef, and renaming one with an alias.
// Passes: when `npx jest exercises/01-beginner/ex009_output_function` is green.
//
// output() vs @Output() + EventEmitter: an OutputEmitterRef is not an RxJS Subject and
// not an Observable — no pipe(), no operators, no manual unsubscribe (Angular tears it
// down with the component). You get exactly emit() and subscribe(), which is all an
// output ever needed. Note it is *not* a signal either: outputs are events, not state.
//
// The emitters are already declared so the spec can subscribe. What is missing is the
// alias on `termChanged` and every method body.
//
// Template contract the spec asserts (classes are the query hooks — keep them):
//   <p class="term">{{ term() }}</p>
//   <button class="submit" type="button" (click)="submit()">Search</button>
//   <button class="clear" type="button" (click)="clear()">Clear</button>
@Component({
  selector: "app-search-box",
  standalone: true,
  template: `<p>TODO: render the search box — see the template contract above</p>`,
})
export class SearchBoxComponent {
  readonly term = signal("");

  /** Emits the trimmed search term when a search is actually run. */
  readonly submitted = output<string>();

  /** A payload-free notification — output<void>() emits with no argument at all. */
  readonly cleared = output<void>();

  /** TODO: give this output the public name `changed`. */
  readonly termChanged = output<{ from: string; to: string }>();

  /** Replace the term and announce the transition. A no-op change emits nothing. */
  type(next: string): void {
    throw new Error("TODO: implement type");
  }

  /**
   * Run the search: emit `submitted` with the term trimmed of surrounding whitespace.
   * A blank or whitespace-only term is not a search — emit nothing.
   */
  submit(): void {
    throw new Error("TODO: implement submit");
  }

  /**
   * Empty the term. Announce the transition through `termChanged` — reusing `type` is
   * the tidy way — and then emit `cleared`. An already-empty box emits nothing at all.
   */
  clear(): void {
    throw new Error("TODO: implement clear");
  }
}

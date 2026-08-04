import { Component, Pipe, PipeTransform, signal } from "@angular/core";

// Exercise 057 — an impure pipe, and when it is justified (intermediate).
// Goal:   see what `pure: false` actually costs, and what the alternatives are.
// Drills: pure: false, being called on every change-detection pass, the performance consequence,
//         and the two better answers — a new reference, or a computed.
// Passes: when `npx jest exercises/02-intermediate/ex057_custom_pipe_impure` is green.
//
// `pure: false` tells Angular to call transform() on *every* change-detection pass, whatever the
// inputs. That solves the stale-output problem from exercise 056 — an array mutated in place is
// picked up — and it solves it by doing the work again, every time, forever. In a list of 500 rows
// inside a component that renders on every mouse move, that is 500 calls per mouse move.
//
// Angular ships exactly one impure pipe of note, AsyncPipe, and it is impure because a subscription
// genuinely can produce a new value at any moment with no input change to observe. That is the bar:
// impure is right when the *source of truth is outside the input list*.
//
// It is almost never the right answer for filtering. Both alternatives are better:
//   - produce a new array when the data changes (a pure pipe then sees it), or
//   - use a computed / getter, which is memoised on its dependencies rather than on nothing.
//
// This exercise implements the impure version so the cost is measurable, and the computed
// alternative beside it so the comparison is concrete rather than asserted.
//
// Template contract the spec asserts (classes are the query hooks — keep them):
//   <p class="impure">{{ items | liveFilter: term() }}</p>
//   <p class="pure">{{ items | staticFilter: term() }}</p>
//   <p class="computed">{{ filtered() }}</p>

/** TODO: the same filter, impure — recalculated on every change-detection pass. */
@Pipe({
  name: "liveFilter",
  standalone: true,
  // TODO: make this impure.
})
export class LiveFilterPipe implements PipeTransform {
  static calls = 0;

  /** Keep the items whose text contains `term`, case-insensitively, joined with ", ". */
  transform(items: readonly string[], term: string): string {
    throw new Error("TODO: implement transform");
  }
}

/** The same logic, left pure, for contrast. */
@Pipe({
  name: "staticFilter",
  standalone: true,
})
export class StaticFilterPipe implements PipeTransform {
  static calls = 0;

  transform(items: readonly string[], term: string): string {
    throw new Error("TODO: implement transform");
  }
}

@Component({
  selector: "app-live-filter-host",
  standalone: true,
  // TODO: import both pipes.
  template: `<p>TODO: render the filters — see the template contract above</p>`,
})
export class LiveFilterHostComponent {
  /**
   * A plain mutable array, deliberately not a signal.
   *
   * This is the situation an impure pipe is usually reached for: data that changes without the
   * reference changing.
   */
  items: string[] = ["apple", "banana", "cherry"];

  readonly term = signal("a");

  /** How many times the computed alternative recalculated. */
  computedCalls = 0;

  /**
   * TODO: the computed alternative.
   *
   * Same result as the pipes, but memoised on `term` — so it recalculates when the term changes
   * and not otherwise. Increment `computedCalls` when the body runs.
   *
   * Declared as a plain method so the stub compiles; the solution makes it a computed.
   */
  filtered(): string {
    throw new Error("TODO: implement filtered");
  }

  /** Push an item without replacing the array. */
  pushInPlace(item: string): void {
    throw new Error("TODO: implement pushInPlace");
  }
}

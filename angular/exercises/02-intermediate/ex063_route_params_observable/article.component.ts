import { Component, signal } from "@angular/core";
import { Observable } from "rxjs";

// Exercise 063 — route parameters as a stream (intermediate).
// Goal:   react to a route change that reuses the component instance.
// Drills: route.paramMap as an Observable, switchMap to reload, the bug snapshot cannot fix, and
//         tearing the subscription down.
// Passes: when `npx jest exercises/02-intermediate/ex063_route_params_observable` is green.
//
// Exercise 062's snapshot is the route as it was when the component was created. Navigate from
// /article/1 to /article/2 and the router *reuses* the component — same instance, no new
// constructor, no new ngOnInit — so a snapshot read once still says "1" and the page shows the
// wrong article with nothing in the console.
//
// paramMap as a stream fixes it because it emits again on every navigation that keeps the
// component alive. Pair it with switchMap and the reload cancels itself: navigating twice quickly
// abandons the first fetch instead of letting it land last (exercise 050).
//
// The subscription needs tearing down like any other. takeUntilDestroyed() is the tidy way, and it
// needs an injection context — which a field initialiser is, and a method is not. Capture a
// DestroyRef in a field and pass it in explicitly, and the operator works from anywhere. Storing
// the operator itself in a field does not work as well as it looks: with nothing to infer from, it
// is typed MonoTypeOperatorFunction<unknown> and widens every pipeline it is dropped into.
//
// Template contract the spec asserts (classes are the query hooks — keep them):
//   <h2 class="title">{{ title() }}</h2>
//   <p class="loads">{{ loadCount() }}</p>

@Component({
  selector: "app-article",
  standalone: true,
  template: `<p>TODO: render the article — see the template contract above</p>`,
})
export class ArticleComponent {
  /** TODO: inject ActivatedRoute and ArticleLoader. */

  /** The loaded article's title, or "" before the first load. */
  readonly title = signal("");

  /** How many loads have completed. */
  readonly loadCount = signal(0);

  /** The ids seen from the route, in order — proof that the stream keeps emitting. */
  readonly seenIds = signal<readonly string[]>([]);

  /**
   * TODO: start following the route.
   *
   * On every paramMap emission: record the id in `seenIds`, then load that article with the
   * loader, switching away from any load still in flight. Each completed load sets `title` and
   * bumps `loadCount`.
   *
   * An emission with no id records "" and loads nothing.
   */
  start(): void {
    throw new Error("TODO: implement start");
  }

  /**
   * TODO: the same, but reading the snapshot once.
   *
   * Kept so the spec can show it going stale across a navigation.
   */
  startFromSnapshot(): void {
    throw new Error("TODO: implement startFromSnapshot");
  }
}

import { Component } from "@angular/core";

// Exercise 062 — reading route parameters from the snapshot (intermediate).
// Goal:   get the current route's parameters without subscribing to anything.
// Drills: ActivatedRoute, snapshot.paramMap / queryParamMap, get vs getAll vs has, the
//         everything-is-a-string rule, and the case where a snapshot is the wrong tool.
// Passes: when `npx jest exercises/02-intermediate/ex062_route_params_snapshot` is green.
//
// `route.snapshot` is the route as it was when the component was created. Simple, synchronous, and
// correct exactly when the component is destroyed and rebuilt on every navigation — which is the
// default.
//
// The trap is the case where it is not. Navigating from /product/1 to /product/2 reuses the same
// component instance, so the snapshot read in ngOnInit still says "1" and the page shows the wrong
// product with no error anywhere. Exercise 063 covers the stream that fixes it; knowing *which*
// situation you are in is the point of having both.
//
// Everything in a paramMap is a string. `snapshot.paramMap.get("id")` for /product/42 is "42", not
// 42, and it is `null` — not undefined — when absent. Converting is the caller's job, and doing it
// carelessly turns a missing parameter into NaN rather than an error.
//
// getAll exists because a query parameter can repeat: ?tag=a&tag=b. `get` returns only the first,
// which quietly loses the rest.
//
// Note the template renders rawId(), not productId(). A template expression that can throw takes
// the whole render down, so an accessor that rejects bad input needs a safe counterpart for
// display — which is also why title() catches rather than propagating.
//
// Template contract the spec asserts (classes are the query hooks — keep them):
//   <h2 class="title">{{ title() }}</h2>
//   <p class="id">{{ rawId() }}</p>
//   <p class="page">{{ page() }}</p>
//   <p class="tags">{{ tags().join(",") }}</p>

@Component({
  selector: "app-product-detail",
  standalone: true,
  template: `<p>TODO: render the detail — see the template contract above</p>`,
})
export class ProductDetailComponent {
  /** TODO: inject ActivatedRoute. */

  /**
   * The `id` route parameter as a number.
   *
   * A missing parameter is a RangeError rather than NaN, and so is one that is not a number.
   */
  productId(): number {
    throw new Error("TODO: implement productId");
  }

  /** The raw `id` parameter, exactly as the router has it — null when absent. */
  rawId(): string | null {
    throw new Error("TODO: implement rawId");
  }

  /** The `page` query parameter as a number, defaulting to 1 when absent or unparseable. */
  page(): number {
    throw new Error("TODO: implement page");
  }

  /** Every `tag` query parameter, in order. An absent one gives []. */
  tags(): readonly string[] {
    throw new Error("TODO: implement tags");
  }

  /** Whether a named query parameter is present at all, even if empty. */
  hasQueryParam(name: string): boolean {
    throw new Error("TODO: implement hasQueryParam");
  }

  /** `"Product <id>"`, or "Unknown product" when there is no usable id. */
  title(): string {
    throw new Error("TODO: implement title");
  }
}

import { Component, signal } from "@angular/core";

// Exercise 064 — navigating from code (intermediate).
// Goal:   send the user somewhere else, with parameters, and know what you asked for.
// Drills: Router.navigate with a command array, queryParams and queryParamsHandling,
//         navigateByUrl, replaceUrl, and reading the boolean the navigation resolves to.
// Passes: when `npx jest exercises/02-intermediate/ex064_router_navigation` is green.
//
// `navigate` takes an *array of commands*, not a URL string: `["/product", 42]` rather than
// `"/product/42"`. The array is the point — segments are joined and encoded for you, so an id
// containing a slash or a space cannot break the URL. `navigateByUrl` takes the string, and is for
// when you genuinely have a whole URL already, such as one that came from an API.
//
// It returns a Promise<boolean>, and the boolean matters: false means the navigation was rejected —
// a guard refused, or a redirect superseded it. Code that assumes success and updates local state
// straight afterwards ends up disagreeing with the URL.
//
// `queryParamsHandling` decides what happens to the parameters already there: "merge" keeps them
// and adds yours, "preserve" keeps theirs and ignores yours, and the default drops them entirely.
// Dropping them is the surprise — a navigation that loses the user's filters is usually this option
// left unset.
//
// Template contract the spec asserts (classes are the query hooks — keep them):
//   <button class="home" type="button" (click)="goHome()">Home</button>
//   <button class="product" type="button" (click)="goToProduct(42)">Product</button>
//   <p class="last">{{ lastResult() }}</p>

@Component({
  selector: "app-nav-bar",
  standalone: true,
  template: `<p>TODO: render the nav bar — see the template contract above</p>`,
})
export class NavBarComponent {
  /** TODO: inject Router. */

  /** "" before any navigation, then "ok" or "rejected". */
  readonly lastResult = signal("");

  /** Navigate to ["/"]. */
  goHome(): void {
    throw new Error("TODO: implement goHome");
  }

  /**
   * Navigate to a product by id, as commands rather than a built string.
   *
   * Records "ok" or "rejected" in `lastResult` once the promise settles.
   */
  goToProduct(id: number | string): void {
    throw new Error("TODO: implement goToProduct");
  }

  /** Navigate to /search with a `q` parameter, dropping any parameters already present. */
  search(term: string): void {
    throw new Error("TODO: implement search");
  }

  /** Navigate to /search with a `q` parameter, keeping the existing ones as well. */
  searchKeepingFilters(term: string): void {
    throw new Error("TODO: implement searchKeepingFilters");
  }

  /** Navigate to a whole URL that arrived as a string. */
  followUrl(url: string): void {
    throw new Error("TODO: implement followUrl");
  }

  /**
   * Go to page `n` of the current route.
   *
   * Stay on this route, keep the other parameters, and replace the history entry rather than
   * adding one — paging should not fill the back button.
   */
  goToPage(n: number): void {
    throw new Error("TODO: implement goToPage");
  }
}

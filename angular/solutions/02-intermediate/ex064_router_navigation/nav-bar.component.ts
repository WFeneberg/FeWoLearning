import { Component, inject, signal } from "@angular/core";
import { Router } from "@angular/router";

// Exercise 064 — navigating from code (reference solution).
@Component({
  selector: "app-nav-bar",
  standalone: true,
  template: `
    <button class="home" type="button" (click)="goHome()">Home</button>
    <button class="product" type="button" (click)="goToProduct(42)">Product</button>
    <p class="last">{{ lastResult() }}</p>
  `,
})
export class NavBarComponent {
  private readonly router = inject(Router);

  readonly lastResult = signal("");

  goHome(): void {
    void this.router.navigate(["/"]);
  }

  goToProduct(id: number | string): void {
    // Commands, not a template string: the router joins and encodes the segments, so an id with a
    // slash or a space cannot break the URL.
    void this.router.navigate(["/product", id]).then((ok) => {
      // The boolean is not decoration — false means a guard or a redirect refused.
      this.lastResult.set(ok ? "ok" : "rejected");
    });
  }

  search(term: string): void {
    // No queryParamsHandling, so existing parameters are discarded. That is the default, and it is
    // worth being deliberate about rather than inheriting by accident.
    void this.router.navigate(["/search"], { queryParams: { q: term } });
  }

  searchKeepingFilters(term: string): void {
    void this.router.navigate(["/search"], {
      queryParams: { q: term },
      queryParamsHandling: "merge",
    });
  }

  followUrl(url: string): void {
    // For a URL that arrived whole: nothing to assemble, so nothing to encode.
    void this.router.navigateByUrl(url);
  }

  goToPage(n: number): void {
    void this.router.navigate([], {
      // Empty commands: stay on this route and change only the parameters.
      queryParams: { page: n },
      queryParamsHandling: "merge",
      // Paging should not fill the back button with every page the user glanced at.
      replaceUrl: true,
    });
  }
}

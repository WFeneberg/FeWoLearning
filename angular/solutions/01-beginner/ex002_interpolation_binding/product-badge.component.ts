import { Component } from "@angular/core";

// Exercise 002 — ProductBadgeComponent (reference solution).
@Component({
  selector: "app-product-badge",
  standalone: true,
  template: `
    <h2 class="label">{{ label }}</h2>
    <p class="stock">{{ stockLabel() }}</p>
    <!-- [disabled] is a property binding: it sets the element's DOM property. -->
    <button class="buy" type="button" [disabled]="soldOut" [attr.data-tone]="tone">Buy</button>
    <!-- data-badge has no DOM property, so it needs [attr.…] — and null removes it. -->
    <a class="details" [href]="detailsUrl" [attr.data-badge]="badge">Details</a>
  `,
})
export class ProductBadgeComponent {
  label = "Widget";
  stock = 3;
  soldOut = false;
  tone = "info";
  detailsUrl = "/products/widget";
  badge: string | null = "new";

  stockLabel(): string {
    if (this.soldOut) {
      return "Sold out";
    }
    return `${this.stock} left`;
  }
}

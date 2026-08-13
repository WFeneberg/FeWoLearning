import { ChangeDetectionStrategy, Component } from "@angular/core";

// Exercise 002 — ProductBadgeComponent (reference solution).
//
// changeDetection is explicit here because Angular 22.1.1's JIT compiler only emits a
// `changeDetection` field on the compiled component definition when metadata.changeDetection
// is an explicit non-OnPush value; an omitted decorator property is compiled as OnPush
// instead of the intended CheckAlways default (see @angular/compiler's
// compileComponentFromMetadata). Every binding here reads a plain, non-signal field, so
// without this the view stops refreshing after its first change-detection pass.
@Component({
  selector: "app-product-badge",
  standalone: true,
  changeDetection: ChangeDetectionStrategy.Default,
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

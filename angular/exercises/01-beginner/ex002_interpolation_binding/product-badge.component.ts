import { Component } from "@angular/core";

// Exercise 002 — ProductBadgeComponent (beginner).
// Goal:   render a product badge using the three ways Angular puts data in the DOM.
// Drills: {{ }} interpolation, [prop] property binding, [attr.x] attribute binding,
//         and the rule that an attribute binding of null *removes* the attribute.
// Passes: when `npx jest exercises/01-beginner/ex002_interpolation_binding` is green.
//
// Why three forms and not one: `[disabled]` sets the DOM *property* of the element,
// which is what actually disables a button. `data-*` and most `aria-*` names have no
// matching property, so they need `[attr.…]` — and there, a null value removes the
// attribute entirely rather than writing the string "null".
//
// Template contract the spec asserts (classes are the query hooks — keep them):
//   <h2 class="label">{{ label }}</h2>
//   <p class="stock">{{ stockLabel() }}</p>
//   <button class="buy" type="button" [disabled]="soldOut" [attr.data-tone]="tone">Buy</button>
//   <a class="details" [href]="detailsUrl" [attr.data-badge]="badge">Details</a>
@Component({
  selector: "app-product-badge",
  standalone: true,
  template: `<p>TODO: render the badge — see the template contract above</p>`,
})
export class ProductBadgeComponent {
  label = "Widget";
  stock = 3;
  soldOut = false;
  tone = "info";
  detailsUrl = "/products/widget";
  /** Null means "no badge", and must leave the data-badge attribute off the element. */
  badge: string | null = "new";

  /** "Sold out" when soldOut, otherwise "1 left" / "3 left". */
  stockLabel(): string {
    throw new Error("TODO: implement stockLabel");
  }
}

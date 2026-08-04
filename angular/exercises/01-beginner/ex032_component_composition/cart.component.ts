import { Component, output, signal } from "@angular/core";

// Exercise 032 — composing components (beginner).
// Goal:   build a three-level tree where data flows down and events come back up.
// Drills: nesting standalone components, passing inputs down through a middle layer,
//         re-emitting a child's output from a parent, and the container/presentational split.
// Passes: when `npx jest exercises/01-beginner/ex032_component_composition` is green.
//
// The shape to internalise: only the outermost component owns state. Everything below it
// takes inputs and emits outputs, which makes the inner two reusable and trivially
// testable — a PriceTag that fetched its own data could only ever be used one way.
//
// The middle layer is where composition gets interesting. CartRow does not own the cart, so
// when its remove button is pressed it cannot do the removing; it re-emits upwards and lets
// the owner decide. That "events travel up one level at a time" discipline is what keeps a
// deep tree from turning into a web of components reaching into each other.
//
// Note the leaf takes an `amount` *number*, not a CartLine. Keeping the presentational
// component ignorant of the domain type is what lets it be reused for a total, a subtotal or
// a tax line without change.
//
// Template contracts the spec asserts (classes are the query hooks — keep them):
//
// PriceTagComponent:
//   <span class="price">{{ formatted() }}</span>
//
// CartRowComponent:
//   <div class="row">
//     <span class="sku">{{ line().sku }}</span>
//     <span class="qty">×{{ line().qty }}</span>
//     <app-price-tag [amount]="lineTotal()" [currency]="currency()" />
//     <button class="remove" type="button" (click)="requestRemoval()">Remove</button>
//   </div>
//
// CartComponent:
//   <div class="cart">
//     @for (line of lines(); track line.sku) {
//       <app-cart-row [line]="line" [currency]="currency()" (removed)="remove($event)" />
//     }
//     <p class="total">Total: <app-price-tag [amount]="total()" [currency]="currency()" /></p>
//     <p class="count">{{ lines().length }} lines</p>
//   </div>

export interface CartLine {
  readonly sku: string;
  readonly qty: number;
  readonly unitPrice: number;
}

@Component({
  selector: "app-price-tag",
  standalone: true,
  template: `<p>TODO: render the price — see the template contract above</p>`,
})
export class PriceTagComponent {
  /**
   * TODO: a required numeric input, and an optional `currency` input defaulting to "EUR".
   *
   * Declared as plain signals so the stub compiles — replace both declarations.
   */
  readonly amount = signal(0);
  readonly currency = signal("EUR");

  /** TODO: the amount fixed to two decimals, a space, then the currency: "12.50 EUR". */
  formatted(): string {
    throw new Error("TODO: implement formatted");
  }
}

@Component({
  selector: "app-cart-row",
  standalone: true,
  // TODO: import PriceTagComponent.
  template: `<p>TODO: render the row — see the template contract above</p>`,
})
export class CartRowComponent {
  /** TODO: a required `line` input and an optional `currency` input defaulting to "EUR". */
  readonly line = signal<CartLine>({ sku: "", qty: 0, unitPrice: 0 });
  readonly currency = signal("EUR");

  /** Emits the sku when this row asks to be removed. Already declared — see exercise 009. */
  readonly removed = output<string>();

  /** qty × unitPrice. */
  lineTotal(): number {
    throw new Error("TODO: implement lineTotal");
  }

  /**
   * Ask to be removed.
   *
   * This row does not own the cart, so it cannot remove itself — it emits and lets the
   * owner act.
   */
  requestRemoval(): void {
    throw new Error("TODO: implement requestRemoval");
  }
}

@Component({
  selector: "app-cart",
  standalone: true,
  // TODO: import CartRowComponent and PriceTagComponent.
  template: `<p>TODO: render the cart — see the template contract above</p>`,
})
export class CartComponent {
  readonly lines = signal<readonly CartLine[]>([
    { sku: "pen", qty: 3, unitPrice: 2 },
    { sku: "pad", qty: 1, unitPrice: 4.5 },
  ]);

  readonly currency = signal("EUR");

  /** The sum of every line total. */
  total(): number {
    throw new Error("TODO: implement total");
  }

  /** Drop a line by sku, immutably. An unknown sku is a no-op. */
  remove(sku: string): void {
    throw new Error("TODO: implement remove");
  }
}

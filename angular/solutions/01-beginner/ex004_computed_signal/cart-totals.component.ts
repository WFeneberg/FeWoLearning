import { Component, computed, signal } from "@angular/core";

// Exercise 004 — CartTotalsComponent (reference solution).

export interface CartLine {
  readonly name: string;
  readonly price: number;
  readonly qty: number;
}

@Component({
  selector: "app-cart-totals",
  standalone: true,
  template: `
    <p class="count">Items: {{ itemCount() }}</p>
    <p class="subtotal">Subtotal: {{ subtotal() }}</p>
    <p class="tax">Tax: {{ tax() }}</p>
    <p class="total">Total: {{ total() }}</p>
  `,
})
export class CartTotalsComponent {
  readonly lines = signal<readonly CartLine[]>([]);
  readonly taxRate = signal(0.2);

  subtotalEvaluations = 0;

  readonly subtotal = computed<number>(() => {
    // A plain field, not a signal: Angular rejects signal writes inside a computed.
    this.subtotalEvaluations += 1;
    return this.lines().reduce((sum, line) => sum + line.price * line.qty, 0);
  });

  // Reading subtotal() here registers it as a dependency, so `tax` re-runs when the
  // lines change — but subtotal itself never reads taxRate and stays cached when the
  // rate moves.
  readonly tax = computed<number>(() => this.subtotal() * this.taxRate());

  readonly total = computed<number>(() => this.subtotal() + this.tax());

  readonly itemCount = computed<number>(() =>
    this.lines().reduce((count, line) => count + line.qty, 0),
  );

  readonly isEmpty = computed<boolean>(() => this.lines().length === 0);

  addLine(line: CartLine): void {
    // A new array, not push(): a signal compares by reference, so mutating in place
    // would leave every computed thinking nothing had changed.
    this.lines.update((lines) => [...lines, line]);
  }
}

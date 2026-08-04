import { Component, computed, signal } from "@angular/core";

// Exercise 004 — CartTotalsComponent (beginner).
// Goal:   derive every total from the cart lines instead of storing it.
// Drills: computed(), laziness (the body does not run until something reads it),
//         memoisation (repeat reads are free), and dependency tracking (a computed
//         re-runs only when a signal it actually read has changed).
// Passes: when `npx jest exercises/01-beginner/ex004_computed_signal` is green.
//
// Why derived state beats stored state: nothing can forget to update `total`, because
// there is no `total` to update. `subtotalEvaluations` exists purely so the spec can
// watch the machinery — note it is a plain field, not a signal, because Angular
// forbids writing to a signal from inside a computed.
//
// Template contract the spec asserts (classes are the query hooks — keep them):
//   <p class="count">Items: {{ itemCount() }}</p>
//   <p class="subtotal">Subtotal: {{ subtotal() }}</p>
//   <p class="tax">Tax: {{ tax() }}</p>
//   <p class="total">Total: {{ total() }}</p>

export interface CartLine {
  readonly name: string;
  readonly price: number;
  readonly qty: number;
}

@Component({
  selector: "app-cart-totals",
  standalone: true,
  template: `<p>TODO: render the totals — see the template contract above</p>`,
})
export class CartTotalsComponent {
  readonly lines = signal<readonly CartLine[]>([]);
  readonly taxRate = signal(0.2);

  /** Bumped once per real evaluation of `subtotal`, so the spec can prove memoisation. */
  subtotalEvaluations = 0;

  /** Sum of price × qty over every line. Increment `subtotalEvaluations` when it runs. */
  readonly subtotal = computed<number>(() => {
    throw new Error("TODO: implement the subtotal computed");
  });

  /** subtotal × taxRate. */
  readonly tax = computed<number>(() => {
    throw new Error("TODO: implement the tax computed");
  });

  /** subtotal + tax. */
  readonly total = computed<number>(() => {
    throw new Error("TODO: implement the total computed");
  });

  /** Total quantity across all lines — not the number of lines. */
  readonly itemCount = computed<number>(() => {
    throw new Error("TODO: implement the itemCount computed");
  });

  /** True when there is nothing in the cart. */
  readonly isEmpty = computed<boolean>(() => {
    throw new Error("TODO: implement the isEmpty computed");
  });

  /** Append a line without mutating the existing array — signals compare by reference. */
  addLine(line: CartLine): void {
    throw new Error("TODO: implement addLine");
  }
}

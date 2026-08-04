import { Component, input, output, signal } from "@angular/core";

// Exercise 032 — composing components (reference solution).

export interface CartLine {
  readonly sku: string;
  readonly qty: number;
  readonly unitPrice: number;
}

@Component({
  selector: "app-price-tag",
  standalone: true,
  template: `<span class="price">{{ formatted() }}</span>`,
})
export class PriceTagComponent {
  // A number, not a CartLine: the leaf knows nothing about carts, so it can be reused for
  // a line, a subtotal or a tax row.
  readonly amount = input.required<number>();
  readonly currency = input("EUR");

  formatted(): string {
    return `${this.amount().toFixed(2)} ${this.currency()}`;
  }
}

@Component({
  selector: "app-cart-row",
  standalone: true,
  imports: [PriceTagComponent],
  template: `
    <div class="row">
      <span class="sku">{{ line().sku }}</span>
      <span class="qty">×{{ line().qty }}</span>
      <app-price-tag [amount]="lineTotal()" [currency]="currency()" />
      <button class="remove" type="button" (click)="requestRemoval()">Remove</button>
    </div>
  `,
})
export class CartRowComponent {
  readonly line = input.required<CartLine>();
  readonly currency = input("EUR");

  readonly removed = output<string>();

  lineTotal(): number {
    const { qty, unitPrice } = this.line();
    return qty * unitPrice;
  }

  requestRemoval(): void {
    // It does not own `lines`, so it cannot splice itself out — it announces and the owner
    // decides. That is what keeps a deep tree from becoming a web.
    this.removed.emit(this.line().sku);
  }
}

@Component({
  selector: "app-cart",
  standalone: true,
  imports: [CartRowComponent, PriceTagComponent],
  template: `
    <div class="cart">
      @for (line of lines(); track line.sku) {
        <app-cart-row [line]="line" [currency]="currency()" (removed)="remove($event)" />
      }
      <p class="total">Total: <app-price-tag [amount]="total()" [currency]="currency()" /></p>
      <p class="count">{{ lines().length }} lines</p>
    </div>
  `,
})
export class CartComponent {
  // The only component in the tree that owns state.
  readonly lines = signal<readonly CartLine[]>([
    { sku: "pen", qty: 3, unitPrice: 2 },
    { sku: "pad", qty: 1, unitPrice: 4.5 },
  ]);

  readonly currency = signal("EUR");

  total(): number {
    return this.lines().reduce((sum, line) => sum + line.qty * line.unitPrice, 0);
  }

  remove(sku: string): void {
    this.lines.update((lines) => lines.filter((line) => line.sku !== sku));
  }
}

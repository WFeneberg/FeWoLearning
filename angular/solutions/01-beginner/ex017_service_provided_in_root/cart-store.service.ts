import { Injectable, signal } from "@angular/core";

// Exercise 017 — CartStore (reference solution).

export interface CartLine {
  readonly sku: string;
  readonly qty: number;
}

// The class names its own provider, so it is both a singleton and tree-shakable.
@Injectable({ providedIn: "root" })
export class CartStore {
  private readonly lines = signal<readonly CartLine[]>([]);

  items(): readonly CartLine[] {
    return this.lines();
  }

  count(): number {
    return this.lines().reduce((total, line) => total + line.qty, 0);
  }

  add(sku: string, qty = 1): void {
    if (qty < 1) {
      throw new RangeError("qty must be at least 1");
    }
    this.lines.update((lines) => {
      const index = lines.findIndex((line) => line.sku === sku);
      if (index === -1) {
        return [...lines, { sku, qty }];
      }
      // map() rather than a splice: the position is preserved and the array is new.
      return lines.map((line, i) =>
        i === index ? { sku: line.sku, qty: line.qty + qty } : line,
      );
    });
  }

  remove(sku: string): void {
    // filter() over a missing sku is already a no-op — no special case needed.
    this.lines.update((lines) => lines.filter((line) => line.sku !== sku));
  }

  clear(): void {
    this.lines.set([]);
  }
}

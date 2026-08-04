import { Injectable, Signal, computed, signal } from "@angular/core";

// Exercise 067 — a signal-based store service (reference solution).

export interface BasketLine {
  readonly sku: string;
  readonly qty: number;
  readonly unitPrice: number;
}

export interface BasketState {
  readonly lines: readonly BasketLine[];
  readonly voucher: string | null;
}

export const EMPTY_BASKET: BasketState = { lines: [], voucher: null };

const emptyBasket = (): BasketState => ({ lines: [], voucher: null });

const round2 = (value: number): number => Math.round(value * 100) / 100;

@Injectable({ providedIn: "root" })
export class BasketStore {
  // One writable signal for the whole state. Two separate signals that must agree could be
  // observed disagreeing mid-update; a single set is atomic.
  private readonly writableState = signal<BasketState>(emptyBasket());

  // asReadonly() is the seal: same signal, no set or update for anyone outside.
  readonly state: Signal<BasketState> = this.writableState.asReadonly();

  readonly lines: Signal<readonly BasketLine[]> = computed(() => this.state().lines);

  readonly itemCount: Signal<number> = computed(() =>
    this.lines().reduce((total, line) => total + line.qty, 0),
  );

  readonly subtotal: Signal<number> = computed(() =>
    round2(this.lines().reduce((total, line) => total + line.qty * line.unitPrice, 0)),
  );

  readonly discount: Signal<number> = computed(() =>
    this.state().voucher === "SAVE10" ? round2(this.subtotal() * 0.1) : 0,
  );

  readonly total: Signal<number> = computed(() => round2(this.subtotal() - this.discount()));

  readonly isEmpty: Signal<boolean> = computed(() => this.lines().length === 0);

  add(sku: string, qty: number, unitPrice: number): void {
    if (qty < 1) {
      throw new RangeError("qty must be at least 1");
    }
    this.writableState.update((current) => {
      const index = current.lines.findIndex((line) => line.sku === sku);
      const lines =
        index === -1
          ? [...current.lines, { sku, qty, unitPrice }]
          : // map() keeps the position and leaves every other line the same object.
            current.lines.map((line, i) =>
              i === index ? { ...line, qty: line.qty + qty } : line,
            );
      return { ...current, lines };
    });
  }

  remove(sku: string): void {
    this.writableState.update((current) => ({
      ...current,
      // filter() over a missing sku already changes nothing.
      lines: current.lines.filter((line) => line.sku !== sku),
    }));
  }

  setQuantity(sku: string, qty: number): void {
    if (qty < 0) {
      throw new RangeError("qty must not be negative");
    }
    if (qty === 0) {
      this.remove(sku);
      return;
    }
    this.writableState.update((current) => ({
      ...current,
      lines: current.lines.map((line) => (line.sku === sku ? { ...line, qty } : line)),
    }));
  }

  applyVoucher(code: string): void {
    const normalised = code.trim().toUpperCase();
    this.writableState.update((current) => ({
      ...current,
      voucher: normalised === "" ? null : normalised,
    }));
  }

  clear(): void {
    // A fresh object, never the shared constant — a later mutation must not be able to reach it.
    this.writableState.set(emptyBasket());
  }

  isSealed(): boolean {
    const asWritable = this.state as unknown as { set?: unknown; update?: unknown };
    return asWritable.set === undefined && asWritable.update === undefined;
  }
}

import { Injectable, Signal, signal } from "@angular/core";

// Exercise 067 — a signal-based store service (intermediate).
// Goal:   hold shared state in a service, exposing reads and commands but never the writer.
// Drills: a private WritableSignal behind public Signals, computed projections, commands as the
//         only mutation path, and asReadonly() as the seal.
// Passes: when `npx jest exercises/02-intermediate/ex067_signal_service_store` is green.
//
// This is exercise 053's discipline in signal form. The store owns one writable signal; the outside
// world gets read-only Signals and methods. `asReadonly()` is the seal — same signal, no set or
// update — and it matters for the same reason asObservable() did: hand out the writable one and the
// store no longer owns its own invariants, so "who emptied the basket?" becomes unanswerable.
//
// One writable signal holding a whole state object beats several loose ones. Two signals that must
// agree can be observed disagreeing halfway through an update; one object cannot, because a single
// set is atomic. Everything else is a computed over it.
//
// The commands are where the rules live. `add` merging a duplicate line rather than appending, and
// refusing a quantity below one, are invariants of the basket — putting them in the store means
// every caller gets them, and no component can forget.
//
// Note what is *not* here: no effect, and no signal written from a computed. Reads derive, commands
// write, and nothing else does either.

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

@Injectable({ providedIn: "root" })
export class BasketStore {
  /**
   * TODO: the single private writable signal holding the whole state, starting from a *copy* of
   * EMPTY_BASKET.
   *
   * A copy, so nothing can reach back and corrupt the shared constant (exercise 031).
   */

  /** TODO: the state, read-only. */
  readonly state: Signal<BasketState> = signal(EMPTY_BASKET);

  /** TODO: the lines, as a computed. */
  readonly lines: Signal<readonly BasketLine[]> = signal([]);

  /** TODO: total quantity across all lines. */
  readonly itemCount: Signal<number> = signal(0);

  /** TODO: the sum of qty × unitPrice. */
  readonly subtotal: Signal<number> = signal(0);

  /** TODO: 10% off when the voucher is "SAVE10", otherwise 0. Rounded to two decimals. */
  readonly discount: Signal<number> = signal(0);

  /** TODO: subtotal minus discount, rounded to two decimals. */
  readonly total: Signal<number> = signal(0);

  /** TODO: whether the basket has nothing in it. */
  readonly isEmpty: Signal<boolean> = signal(true);

  /** Add a quantity of a sku, merging with an existing line. A qty below 1 is a RangeError. */
  add(sku: string, qty: number, unitPrice: number): void {
    throw new Error("TODO: implement add");
  }

  /** Remove a sku entirely. An unknown sku is a no-op. */
  remove(sku: string): void {
    throw new Error("TODO: implement remove");
  }

  /** Set a sku's quantity. Zero removes the line; below zero is a RangeError. */
  setQuantity(sku: string, qty: number): void {
    throw new Error("TODO: implement setQuantity");
  }

  /** Apply a voucher code, upper-cased and trimmed. A blank code clears it. */
  applyVoucher(code: string): void {
    throw new Error("TODO: implement applyVoucher");
  }

  /** Back to an empty basket. */
  clear(): void {
    throw new Error("TODO: implement clear");
  }

  /**
   * Whether the public state signal is write-sealed.
   *
   * TODO: return true when `state` has neither a `set` nor an `update` method.
   */
  isSealed(): boolean {
    throw new Error("TODO: implement isSealed");
  }
}

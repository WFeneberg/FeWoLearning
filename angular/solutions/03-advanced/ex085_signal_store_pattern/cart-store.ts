import { Injectable, computed, signal } from "@angular/core";

// Exercise 085 — a typed signal store: actions and selectors (reference solution).

export interface CartLine {
  readonly sku: string;
  readonly qty: number;
  readonly unitPrice: number;
}

export interface CartState {
  readonly lines: readonly CartLine[];
}

export const EMPTY_CART: CartState = { lines: [] };

export type CartAction =
  | { readonly type: "add"; readonly sku: string; readonly qty: number; readonly unitPrice: number }
  | { readonly type: "remove"; readonly sku: string }
  | { readonly type: "clear" };

export function reduce(state: CartState, action: CartAction): CartState {
  switch (action.type) {
    case "add": {
      if (action.qty < 1) {
        throw new RangeError("qty must be at least 1");
      }
      const existing = state.lines.find((line) => line.sku === action.sku);
      if (existing) {
        return {
          lines: state.lines.map((line) =>
            line.sku === action.sku
              ? { sku: line.sku, qty: line.qty + action.qty, unitPrice: action.unitPrice }
              : line,
          ),
        };
      }
      return {
        lines: [...state.lines, { sku: action.sku, qty: action.qty, unitPrice: action.unitPrice }],
      };
    }
    case "remove":
      return { lines: state.lines.filter((line) => line.sku !== action.sku) };
    case "clear":
      return EMPTY_CART;
  }
}

@Injectable({ providedIn: "root" })
export class CartStore {
  private readonly state = signal<CartState>(EMPTY_CART);

  readonly lines = computed(() => this.state().lines);
  readonly itemCount = computed(() => this.lines().reduce((sum, line) => sum + line.qty, 0));
  readonly subtotal = computed(() =>
    this.lines().reduce((sum, line) => sum + line.qty * line.unitPrice, 0),
  );
  readonly isEmpty = computed(() => this.lines().length === 0);

  dispatch(action: CartAction): void {
    this.state.update((current) => reduce(current, action));
  }
}

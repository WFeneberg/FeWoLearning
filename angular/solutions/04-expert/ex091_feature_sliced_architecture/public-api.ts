import {
  applyDiscountPercent,
  applyTaxPercent,
  roundToCents,
  subtotalOf,
} from "./internal/cart-lines";

// Exercise 091 — feature-sliced architecture: a barrel-free public API boundary (reference solution).

export interface OrderLine {
  readonly quantity: number;
  readonly unitPrice: number;
}

export interface OrderOptions {
  readonly discountPercent?: number;
  readonly taxPercent?: number;
}

export interface OrderTotals {
  readonly subtotal: number;
  readonly total: number;
}

export function calculateOrderTotal(
  lines: readonly OrderLine[],
  options: OrderOptions = {},
): OrderTotals {
  const subtotal = subtotalOf(lines);
  const discounted = applyDiscountPercent(subtotal, options.discountPercent ?? 0);
  const taxed = applyTaxPercent(discounted, options.taxPercent ?? 0);

  return {
    subtotal: roundToCents(subtotal),
    total: roundToCents(taxed),
  };
}

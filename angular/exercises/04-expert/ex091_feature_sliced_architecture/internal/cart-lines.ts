// Exercise 091 — internal implementation detail of the "cart" feature.
//
// Nothing in this file is meant to be imported from outside this feature folder. There is no
// TypeScript compiler rule enforcing that in a plain workspace like this one (that is usually an
// ESLint boundary rule — `eslint-plugin-boundaries`, Nx module boundaries, or similar — layered on
// top of the compiler); the discipline here is a naming and folder convention: anything under
// `internal/` is fair game to reshape or delete the moment it stops matching what `../public-api.ts`
// needs, precisely because nothing outside this folder is supposed to hold a reference to it.
// `public-api.ts` is the one file other features (and this exercise's test) are allowed to import.

export interface InternalCartLine {
  readonly quantity: number;
  readonly unitPrice: number;
}

export function subtotalOf(lines: readonly InternalCartLine[]): number {
  return lines.reduce((sum, line) => sum + line.quantity * line.unitPrice, 0);
}

export function applyDiscountPercent(amount: number, percent: number): number {
  return amount * (1 - percent / 100);
}

export function applyTaxPercent(amount: number, percent: number): number {
  return amount * (1 + percent / 100);
}

/** An internal rounding helper — not part of the feature's public contract. */
export function roundToCents(amount: number): number {
  return Math.round(amount * 100) / 100;
}

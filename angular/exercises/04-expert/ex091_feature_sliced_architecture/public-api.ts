import {
  applyDiscountPercent,
  applyTaxPercent,
  roundToCents,
  subtotalOf,
} from "./internal/cart-lines";

// Exercise 091 — feature-sliced architecture: a barrel-free public API boundary (expert).
// Goal:   expose exactly one seam between a feature and the rest of the app — no `index.ts` barrel
//         re-exporting everything a folder happens to contain, just this one file, with exactly the
//         symbols the feature intends to make load-bearing for anyone else.
// Drills: composing internal helpers behind a stable public function instead of re-exporting them,
//         and keeping the *runtime* export surface exactly as small as the intended contract.
// Passes: when `npx jest exercises/04-expert/ex091_feature_sliced_architecture` is green.
//
// A barrel file (`export * from "./internal/cart-lines"` re-exported through an `index.ts`) is
// convenient exactly because it requires no thought about what should be reachable from outside —
// which is the whole problem. Every internal helper, every implementation-detail constant, becomes
// permanently importable the moment it exists, and once some other feature has quietly started
// depending on `roundToCents` directly, this feature can no longer change *how* it rounds without
// grepping the entire codebase for who else reached in. "Feature-sliced" here means the opposite
// discipline: `internal/` holds whatever this feature needs to get its job done and is free to
// reshape at will; `public-api.ts` is the one place that composes those pieces into a contract
// stated on its own terms (`OrderLine`, `OrderOptions`, `OrderTotals` — not `InternalCartLine`) and
// is the only file anything outside this folder (including this exercise's own spec) is allowed to
// import from.
//
// The order of operations below is itself worth getting right, not just glued together: a discount
// applies to the subtotal, and tax applies on top of the *discounted* amount, not the original
// subtotal — reversing that order (or taxing the pre-discount amount) is a real, easy-to-miss pricing
// bug, the kind a spec that checks the composition (not just each helper in isolation) exists to
// catch.

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

/**
 * TODO: implement calculateOrderTotal — the ONE function this feature exposes.
 *   - `subtotal` = `subtotalOf(lines)`, rounded to cents.
 *   - `total` = subtotal with `options.discountPercent` (default 0) applied, then
 *     `options.taxPercent` (default 0) applied on top of THAT discounted amount — not the original
 *     subtotal — rounded to cents.
 *   - Use `applyDiscountPercent`, `applyTaxPercent` and `roundToCents` from `./internal/cart-lines`;
 *     do not re-export them here, and do not reimplement their arithmetic inline.
 */
export function calculateOrderTotal(
  lines: readonly OrderLine[],
  options: OrderOptions = {},
): OrderTotals {
  throw new Error("TODO: implement calculateOrderTotal");
}

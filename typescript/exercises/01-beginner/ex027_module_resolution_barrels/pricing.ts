// The other half of the cycle — and the one that is broken.
//
// TODO: ./catalogue imports this module, and this module imports ./catalogue.
// That is legal, and ES modules handle it with live bindings — but only if
// nobody READS across the cycle while the other module is still evaluating.
// The line below reads BASE_PRICES at exactly that moment. Move the read
// inside priceOf, where it happens after both modules have finished.
//
// Measured, because the symptom is worse than you would hope: the import
// does NOT throw. `captured` is simply undefined, and nothing complains
// until priceOf is called and the property read fails somewhere else
// entirely. A cycle that half-works is the normal presentation.
import { BASE_PRICES } from "./catalogue";

const captured = BASE_PRICES;

export function priceOf(sku: string): number {
  return captured[sku] ?? 0;
}

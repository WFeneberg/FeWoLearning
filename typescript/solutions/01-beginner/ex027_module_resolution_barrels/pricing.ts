// Reference solution — exercise 027, the cycle half.
// The import stays; only the moment of the READ moves. By the time priceOf
// is called, both modules have finished evaluating and the live binding
// resolves.
import { BASE_PRICES } from "./catalogue";

export function priceOf(sku: string): number {
  return BASE_PRICES[sku] ?? 0;
}

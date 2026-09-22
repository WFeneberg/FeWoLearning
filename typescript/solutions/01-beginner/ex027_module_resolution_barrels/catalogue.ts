// Given, one half of a deliberate import cycle. Do not change.
// It imports ./pricing, and ./pricing imports it back.
import { priceOf } from "./pricing";

export const BASE_PRICES: Record<string, number> = {
  widget: 10,
  gadget: 25,
};

export function describeItem(sku: string): string {
  return `${sku}: ${priceOf(sku)}`;
}

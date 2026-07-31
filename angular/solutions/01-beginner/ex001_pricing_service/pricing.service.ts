import { Injectable } from "@angular/core";

// Exercise 001 — PricingService (reference solution).
@Injectable({ providedIn: "root" })
export class PricingService {
  applyDiscount(price: number, percent: number): number {
    if (percent < 0 || percent > 100) {
      throw new RangeError("percent must be between 0 and 100");
    }
    return price * (1 - percent / 100);
  }
}

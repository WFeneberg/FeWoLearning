import { Injectable } from "@angular/core";

// Exercise 001 — PricingService (beginner).
// Goal:   applyDiscount(price, percent) returns the discounted price.
//         Throw RangeError if percent is outside 0..100.
// Drills: injectable services, DI, argument validation.
@Injectable({ providedIn: "root" })
export class PricingService {
  applyDiscount(_price: number, _percent: number): number {
    throw new Error("TODO: implement applyDiscount");
  }
}

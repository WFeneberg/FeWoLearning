import { TestBed } from "@angular/core/testing";
import { PricingService } from "./pricing.service";

describe("PricingService", () => {
  let service: PricingService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(PricingService);
  });

  it("applies a percentage discount", () => {
    expect(service.applyDiscount(200, 25)).toBe(150);
  });

  it("returns the price unchanged for 0%", () => {
    expect(service.applyDiscount(99.9, 0)).toBeCloseTo(99.9);
  });

  it("throws for out-of-range percentages", () => {
    expect(() => service.applyDiscount(100, -5)).toThrow(RangeError);
    expect(() => service.applyDiscount(100, 150)).toThrow(RangeError);
  });
});

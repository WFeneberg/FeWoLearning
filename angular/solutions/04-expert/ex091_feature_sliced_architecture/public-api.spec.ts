// Deliberately imports ONLY from the feature's public API - never from "./internal/...".
// A test file reaching into "./internal/cart-lines" directly would itself be violating the boundary
// this exercise is about.
import * as cartFeature from "./public-api";

describe("cart feature public API (barrel-free boundary)", () => {
  it("exposes exactly the sanctioned surface, and that surface computes a real result", () => {
    expect(Object.keys(cartFeature).sort()).toEqual(["calculateOrderTotal"]);

    const totals = cartFeature.calculateOrderTotal([{ quantity: 2, unitPrice: 5 }]);
    expect(totals.subtotal).toBe(10);
  });

  it("computes subtotal and total identically when no discount or tax is given", () => {
    const totals = cartFeature.calculateOrderTotal([
      { quantity: 2, unitPrice: 5 },
      { quantity: 1, unitPrice: 3 },
    ]);

    expect(totals.subtotal).toBe(13);
    expect(totals.total).toBe(13);
  });

  it("applies a discount to the subtotal when no tax is given", () => {
    const totals = cartFeature.calculateOrderTotal([{ quantity: 1, unitPrice: 100 }], {
      discountPercent: 10,
    });

    expect(totals.subtotal).toBe(100);
    expect(totals.total).toBe(90);
  });

  it("applies tax on top of the DISCOUNTED amount, not the original subtotal", () => {
    const totals = cartFeature.calculateOrderTotal([{ quantity: 1, unitPrice: 100 }], {
      discountPercent: 50,
      taxPercent: 10,
    });

    // subtotal 100 -> discounted to 50 -> +10% tax on 50 = 55 (NOT 110, NOT 10% of 100 added to 50)
    expect(totals.total).toBe(55);
  });

  it("rounds the total to cents, cleaning up floating point error", () => {
    const totals = cartFeature.calculateOrderTotal([{ quantity: 3, unitPrice: 0.1 }]);

    expect(totals.subtotal).toBe(0.3);
    expect(totals.total).toBe(0.3);
  });
});

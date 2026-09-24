import { describe, expect, it } from "vitest";
import {
  formatMoney,
  isExactInteger,
  nearlyEqual,
  sumAmounts,
} from "@ex/01-beginner/ex004_number_precision/index.js";

describe("ex004 nearlyEqual", () => {
  it("accepts the canonical rounding error", () => {
    expect(0.1 + 0.2 === 0.3).toBe(false);
    expect(nearlyEqual(0.1 + 0.2, 0.3)).toBe(true);
  });

  it("still rejects genuinely different numbers", () => {
    expect(nearlyEqual(1, 1.000001)).toBe(false);
    expect(nearlyEqual(0, 1)).toBe(false);
  });

  it("scales the tolerance with magnitude", () => {
    // One ulp at 1e16 is 2.0. A fixed epsilon of 2.2e-16 would call these
    // different, which is the bug this assertion exists to catch.
    expect(nearlyEqual(1e16, 1e16 + 2)).toBe(true);
    expect(nearlyEqual(1e16, 1e16 + 64)).toBe(false);
  });

  it("honours an explicit epsilon", () => {
    expect(nearlyEqual(1, 1.05, 0.1)).toBe(true);
    expect(nearlyEqual(1, 1.05, 0.001)).toBe(false);
  });

  it("never calls NaN nearly equal, and matches infinities exactly", () => {
    expect(nearlyEqual(NaN, NaN)).toBe(false);
    expect(nearlyEqual(NaN, 1)).toBe(false);
    expect(nearlyEqual(Infinity, Infinity)).toBe(true);
    expect(nearlyEqual(Infinity, -Infinity)).toBe(false);
  });
});

describe("ex004 formatMoney", () => {
  it("always prints two decimals", () => {
    expect(formatMoney(1)).toBe("1.00");
    expect(formatMoney(1.5)).toBe("1.50");
    expect(formatMoney(-2.345)).toBe("-2.35");
    expect(formatMoney(-0)).toBe("0.00");
  });

  it("rounds the way the stored double actually is, not the way it looks", () => {
    // 1.005 is stored slightly BELOW 1.005, so it rounds down. Not a bug in
    // toFixed — a consequence of there being no such double as 1.005.
    expect(formatMoney(1.005)).toBe("1.00");
    expect(formatMoney(1.015)).toBe("1.01");
  });
});

describe("ex004 sumAmounts", () => {
  it("returns an exactly comparable total", () => {
    expect(sumAmounts([0.1, 0.2])).toBe(0.3);
    expect(sumAmounts([0.1, 0.2, 0.3])).toBe(0.6);
    expect(0.1 + 0.2 + 0.3).not.toBe(0.6);
  });

  it("survives a long run of cents", () => {
    expect(sumAmounts(Array.from({ length: 10 }, () => 0.1))).toBe(1);
  });

  it("handles an empty list and negative amounts", () => {
    expect(sumAmounts([])).toBe(0);
    expect(sumAmounts([19.99, -19.99])).toBe(0);
    expect(sumAmounts([10, -0.55])).toBe(9.45);
  });
});

describe("ex004 isExactInteger", () => {
  it("accepts integers inside the safe range", () => {
    expect(isExactInteger(0)).toBe(true);
    expect(isExactInteger(-7)).toBe(true);
    expect(isExactInteger(Number.MAX_SAFE_INTEGER)).toBe(true);
  });

  it("rejects integers past 2^53 - 1, where +1 stops being visible", () => {
    expect(Number.isInteger(2 ** 53)).toBe(true);
    expect(2 ** 53 === 2 ** 53 + 1).toBe(true);
    expect(isExactInteger(2 ** 53)).toBe(false);
  });

  it("rejects non-integers and non-numbers", () => {
    expect(isExactInteger(1.5)).toBe(false);
    expect(isExactInteger(NaN)).toBe(false);
    expect(isExactInteger(Infinity)).toBe(false);
    expect(isExactInteger("3")).toBe(false);
  });
});

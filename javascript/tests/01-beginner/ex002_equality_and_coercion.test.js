import { describe, expect, it } from "vitest";
import {
  dedupe,
  isSameValueZero,
  normalizeZero,
} from "@ex/01-beginner/ex002_equality_and_coercion/index.js";

describe("ex002 isSameValueZero", () => {
  it("agrees with === on ordinary values", () => {
    expect(isSameValueZero(1, 1)).toBe(true);
    expect(isSameValueZero("a", "a")).toBe(true);
    const obj = {};
    expect(isSameValueZero(obj, obj)).toBe(true);
    expect(isSameValueZero({}, {})).toBe(false);
  });

  it("does not coerce, so it is not ==", () => {
    // Every one of these is true under `==`.
    expect(isSameValueZero(1, "1")).toBe(false);
    expect(isSameValueZero(0, false)).toBe(false);
    expect(isSameValueZero(null, undefined)).toBe(false);
    expect(isSameValueZero([], "")).toBe(false);
  });

  it("calls NaN equal to itself, unlike ===", () => {
    expect(NaN === NaN).toBe(false);
    expect(isSameValueZero(NaN, NaN)).toBe(true);
    expect(isSameValueZero(NaN, 0)).toBe(false);
  });

  it("calls +0 and -0 equal, unlike Object.is", () => {
    expect(Object.is(0, -0)).toBe(false);
    expect(isSameValueZero(0, -0)).toBe(true);
  });
});

describe("ex002 dedupe", () => {
  it("keeps first-seen order", () => {
    expect(dedupe(["b", "a", "b", "c", "a"])).toEqual(["b", "a", "c"]);
  });

  it("collapses repeated NaN — which indexOf never would", () => {
    expect([NaN].indexOf(NaN)).toBe(-1);
    expect(dedupe([NaN, 1, NaN])).toEqual([NaN, 1]);
  });

  it("collapses +0 with -0", () => {
    expect(dedupe([0, -0])).toHaveLength(1);
  });

  it("leaves the input untouched and returns a new array", () => {
    const input = [1, 1, 2];
    const result = dedupe(input);
    expect(input).toEqual([1, 1, 2]);
    expect(result).not.toBe(input);
  });
});

describe("ex002 normalizeZero", () => {
  it("turns -0 into 0", () => {
    expect(Object.is(normalizeZero(-0), 0)).toBe(true);
  });

  it("leaves every other number alone", () => {
    expect(Object.is(normalizeZero(0), 0)).toBe(true);
    expect(normalizeZero(-1)).toBe(-1);
    expect(normalizeZero(Infinity)).toBe(Infinity);
    expect(Number.isNaN(normalizeZero(NaN))).toBe(true);
  });
});

import { describe, expect, it } from "vitest";
import { describeValue, isPrimitive } from "@ex/01-beginner/ex001_value_types/index.js";

describe("ex001 describeValue", () => {
  it("names each primitive", () => {
    expect(describeValue("hi")).toBe("string");
    expect(describeValue(42)).toBe("number");
    expect(describeValue(42n)).toBe("bigint");
    expect(describeValue(true)).toBe("boolean");
    expect(describeValue(Symbol("s"))).toBe("symbol");
    expect(describeValue(undefined)).toBe("undefined");
  });

  it("says null rather than object", () => {
    // `typeof null === "object"` is the oldest bug in the language.
    expect(typeof null).toBe("object");
    expect(describeValue(null)).toBe("null");
  });

  it("separates arrays from other objects", () => {
    expect(describeValue([])).toBe("array");
    expect(describeValue([1, 2, 3])).toBe("array");
    expect(describeValue({})).toBe("object");
    expect(describeValue(new Date())).toBe("object");
    expect(describeValue(new Map())).toBe("object");
  });

  it("calls a function a function", () => {
    expect(describeValue(function named() {})).toBe("function");
    expect(describeValue(() => {})).toBe("function");
    expect(describeValue(class C {})).toBe("function");
  });
});

describe("ex001 isPrimitive", () => {
  it("accepts every primitive, null included", () => {
    for (const value of ["", 0, 0n, false, Symbol.iterator, undefined, null]) {
      expect(isPrimitive(value)).toBe(true);
    }
  });

  it("rejects objects and functions, including exotic ones", () => {
    for (const value of [{}, [], new Date(), new Map(), /re/, () => {}, Promise.resolve()]) {
      expect(isPrimitive(value)).toBe(false);
    }
  });
});

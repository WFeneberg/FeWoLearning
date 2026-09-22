import { describe, expect, it } from "vitest";
import { deepFreeze } from "@ex/02-intermediate/ex065_deep_readonly/index";

describe("ex065 deepFreeze", () => {
  it("freezes the top level", () => {
    const value = deepFreeze({ a: 1 });
    expect(Object.isFrozen(value)).toBe(true);
  });

  // Object.freeze alone is shallow, which is the whole reason this
  // function exists.
  it("freezes nested objects too", () => {
    const value = deepFreeze({ a: { b: { c: 1 } } });
    expect(Object.isFrozen(value.a)).toBe(true);
    expect(Object.isFrozen(value.a.b)).toBe(true);
  });

  it("freezes arrays and their contents", () => {
    const value = deepFreeze({ xs: [{ n: 1 }] });
    expect(Object.isFrozen(value.xs)).toBe(true);
    expect(Object.isFrozen(value.xs[0])).toBe(true);
  });

  it("gives back the same object, not a copy", () => {
    const original = { a: 1 };
    expect(deepFreeze(original)).toBe(original);
  });

  it("leaves primitives and null alone without throwing", () => {
    expect(deepFreeze(42)).toBe(42);
    expect(deepFreeze(null)).toBeNull();
    expect(deepFreeze(undefined)).toBeUndefined();
  });

  it("really does prevent a nested write", () => {
    const value = deepFreeze({ a: { b: 1 } });
    expect(() => {
      (value.a as { b: number }).b = 2;
    }).toThrow(TypeError);
  });
});

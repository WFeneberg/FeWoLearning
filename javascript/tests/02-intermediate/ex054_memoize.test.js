import { describe, expect, it, vi } from "vitest";
import { memoize, memoizeBy, memoizeWeak } from "@ex/02-intermediate/ex054_memoize/index.js";

describe("ex054 memoize", () => {
  it("computes once per distinct argument", () => {
    const fn = vi.fn((n) => n * 2);
    const memoized = memoize(fn);
    expect(memoized(2)).toBe(4);
    expect(memoized(2)).toBe(4);
    expect(memoized(3)).toBe(6);
    expect(fn).toHaveBeenCalledTimes(2);
  });

  it("caches a falsy result", () => {
    const fn = vi.fn(() => 0);
    const memoized = memoize(fn);
    memoized("k");
    memoized("k");
    expect(fn).toHaveBeenCalledTimes(1);
  });

  it("caches undefined — the bug a `cache.get() ?? compute()` has", () => {
    const fn = vi.fn(() => undefined);
    const memoized = memoize(fn);
    expect(memoized("k")).toBeUndefined();
    expect(memoized("k")).toBeUndefined();
    expect(fn).toHaveBeenCalledTimes(1);
  });

  it("keys on identity for objects and SameValueZero for NaN", () => {
    const fn = vi.fn((value) => value);
    const memoized = memoize(fn);
    memoized({});
    memoized({});
    expect(fn).toHaveBeenCalledTimes(2);

    const nanFn = vi.fn(() => "nan");
    const memoizedNan = memoize(nanFn);
    memoizedNan(NaN);
    memoizedNan(NaN);
    expect(nanFn).toHaveBeenCalledTimes(1);
  });

  it("recomputes after clear()", () => {
    const fn = vi.fn((n) => n);
    const memoized = memoize(fn);
    memoized(1);
    memoized.clear();
    memoized(1);
    expect(fn).toHaveBeenCalledTimes(2);
  });

  it("gives each memoized function its own cache", () => {
    const fn = vi.fn((n) => n);
    memoize(fn)(1);
    memoize(fn)(1);
    expect(fn).toHaveBeenCalledTimes(2);
  });
});

describe("ex054 memoizeBy", () => {
  it("uses the key function", () => {
    const fn = vi.fn((a, b) => a + b);
    const memoized = memoizeBy(fn, (a, b) => `${a}|${b}`);
    expect(memoized(1, 2)).toBe(3);
    expect(memoized(1, 2)).toBe(3);
    expect(memoized(2, 1)).toBe(3);
    expect(fn).toHaveBeenCalledTimes(2);
  });

  it("collapses arguments the key function calls equal", () => {
    const fn = vi.fn((user) => user.name.toUpperCase());
    const memoized = memoizeBy(fn, (user) => user.id);
    expect(memoized({ id: 1, name: "ada" })).toBe("ADA");
    expect(memoized({ id: 1, name: "different" })).toBe("ADA");
    expect(fn).toHaveBeenCalledTimes(1);
  });

  it("passes the original arguments through, not the key", () => {
    const fn = vi.fn();
    memoizeBy(fn, () => "k")(1, 2);
    expect(fn).toHaveBeenCalledWith(1, 2);
  });
});

describe("ex054 memoizeWeak", () => {
  it("caches per object identity", () => {
    const fn = vi.fn((object) => object.value * 2);
    const memoized = memoizeWeak(fn);
    const key = { value: 5 };
    expect(memoized(key)).toBe(10);
    expect(memoized(key)).toBe(10);
    expect(memoized({ value: 5 })).toBe(10);
    expect(fn).toHaveBeenCalledTimes(2);
  });

  it("refuses a primitive key, because a WeakMap cannot hold one", () => {
    expect(() => memoizeWeak((n) => n)(5)).toThrow(TypeError);
  });

  it("caches a stale result until the key is gone — no invalidation here", () => {
    const fn = vi.fn((object) => object.value);
    const memoized = memoizeWeak(fn);
    const key = { value: 1 };
    expect(memoized(key)).toBe(1);
    key.value = 99;
    expect(memoized(key)).toBe(1);
  });
});

import { describe, expect, it, vi } from "vitest";
import {
  cityOf,
  itemCount,
  notify,
  readKey,
} from "@ex/01-beginner/ex019_optional_chaining/index.js";

describe("ex019 cityOf", () => {
  it("reads the whole path", () => {
    expect(cityOf({ address: { city: "Bern" } })).toBe("Bern");
  });

  it("falls back at any missing level", () => {
    expect(cityOf({ address: {} })).toBe("unknown");
    expect(cityOf({})).toBe("unknown");
    expect(cityOf(null)).toBe("unknown");
    expect(cityOf(undefined)).toBe("unknown");
    expect(cityOf({ address: null })).toBe("unknown");
  });
});

describe("ex019 notify", () => {
  it("calls the handler and returns its result", () => {
    const onEvent = vi.fn(() => "handled");
    expect(notify({ onEvent }, 42)).toBe("handled");
    expect(onEvent).toHaveBeenCalledWith(42);
  });

  it("is a no-op when there is no handler", () => {
    expect(notify({}, 42)).toBeUndefined();
    expect(notify(null, 42)).toBeUndefined();
  });

  it("still throws when the property exists but is not callable", () => {
    // ?.() guards against null/undefined only. A string is neither.
    expect(() => notify({ onEvent: "not a function" }, 1)).toThrow(TypeError);
  });
});

describe("ex019 readKey", () => {
  it("reads the computed key", () => {
    expect(readKey({ a: 1 }, () => "a")).toBe(1);
  });

  it("short-circuits the whole chain — the key function never runs", () => {
    // This is the difference between short-circuiting and defaulting: the
    // expression to the right of ?. is not evaluated at all.
    const keyFn = vi.fn(() => "a");
    expect(readKey(null, keyFn)).toBeUndefined();
    expect(readKey(undefined, keyFn)).toBeUndefined();
    expect(keyFn).not.toHaveBeenCalled();
  });
});

describe("ex019 itemCount", () => {
  it("counts", () => {
    expect(itemCount({ items: ["a", "b"] })).toBe(2);
  });

  it("is 0 for a missing order, missing items or an empty list", () => {
    expect(itemCount({ items: [] })).toBe(0);
    expect(itemCount({})).toBe(0);
    expect(itemCount(null)).toBe(0);
  });
});

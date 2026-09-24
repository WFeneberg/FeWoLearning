import { describe, expect, it } from "vitest";
import {
  cloneShallow,
  maxOf,
  mergeConfig,
  sum,
  withoutKey,
} from "@ex/01-beginner/ex013_rest_and_spread/index.js";

describe("ex013 sum", () => {
  it("takes any arity", () => {
    expect(sum()).toBe(0);
    expect(sum(1)).toBe(1);
    expect(sum(1, 2, 3)).toBe(6);
  });

  it("can be fed an array by spreading at the call site", () => {
    expect(sum(...[1, 2, 3, 4])).toBe(10);
  });

  it("declares a rest parameter, so its length is 0", () => {
    // Anchored to a real call: the stub already declares ...numbers, so the
    // length fact alone would be green before a line of work.
    expect(sum(1, 2, 3)).toBe(6);
    // A rest parameter does not count toward Function.length — which is how
    // you can tell one from three declared parameters.
    expect(sum.length).toBe(0);
  });
});

describe("ex013 maxOf", () => {
  it("finds the max", () => {
    expect(maxOf([3, 9, 1])).toBe(9);
    expect(maxOf([-3, -9])).toBe(-3);
  });

  it("is -Infinity over nothing", () => {
    expect(maxOf([])).toBe(-Infinity);
  });
});

describe("ex013 mergeConfig", () => {
  it("lets the override win", () => {
    expect(mergeConfig({ a: 1, b: 2 }, { b: 3 })).toEqual({ a: 1, b: 3 });
  });

  it("copies an explicit undefined over — spread is not a defaulting tool", () => {
    const merged = mergeConfig({ a: 1 }, { a: undefined });
    expect(merged.a).toBeUndefined();
    expect(Object.hasOwn(merged, "a")).toBe(true);
  });

  it("touches neither input", () => {
    const base = { a: 1 };
    const override = { b: 2 };
    const merged = mergeConfig(base, override);
    expect(base).toEqual({ a: 1 });
    expect(override).toEqual({ b: 2 });
    expect(merged).not.toBe(base);
    expect(merged).not.toBe(override);
  });
});

describe("ex013 withoutKey", () => {
  it("removes the named key", () => {
    expect(withoutKey({ a: 1, b: 2, c: 3 }, "b")).toEqual({ a: 1, c: 3 });
  });

  it("removes a key whose name is only known at runtime", () => {
    const key = ["pass", "word"].join("");
    expect(withoutKey({ user: "ada", password: "x" }, key)).toEqual({ user: "ada" });
  });

  it("is a no-op for a key that is not there", () => {
    expect(withoutKey({ a: 1 }, "zzz")).toEqual({ a: 1 });
  });

  it("does not delete from the original", () => {
    const input = { a: 1, b: 2 };
    expect(withoutKey(input, "a")).not.toBe(input);
    expect(input).toEqual({ a: 1, b: 2 });
  });
});

describe("ex013 cloneShallow", () => {
  it("copies the top level", () => {
    const input = { a: 1, nested: { deep: true } };
    const copy = cloneShallow(input);
    expect(copy).toEqual(input);
    expect(copy).not.toBe(input);
    copy.a = 2;
    expect(input.a).toBe(1);
  });

  it("shares the nested objects — that is what shallow means", () => {
    const input = { nested: { deep: true } };
    const copy = cloneShallow(input);
    expect(copy.nested).toBe(input.nested);
    copy.nested.deep = false;
    expect(input.nested.deep).toBe(false);
  });
});

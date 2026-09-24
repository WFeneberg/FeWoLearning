import { describe, expect, it } from "vitest";
import { deepEqual } from "@ex/02-intermediate/ex056_deep_equal/index.js";

describe("ex056 deepEqual — primitives", () => {
  it("compares like ===", () => {
    expect(deepEqual(1, 1)).toBe(true);
    expect(deepEqual("a", "a")).toBe(true);
    expect(deepEqual(1, "1")).toBe(false);
    expect(deepEqual(null, undefined)).toBe(false);
    expect(deepEqual(0, false)).toBe(false);
  });

  it("calls NaN equal to itself", () => {
    expect(deepEqual(NaN, NaN)).toBe(true);
    expect(deepEqual([NaN], [NaN])).toBe(true);
  });
});

describe("ex056 deepEqual — structures", () => {
  it("compares arrays element-wise", () => {
    expect(deepEqual([1, 2, 3], [1, 2, 3])).toBe(true);
    expect(deepEqual([1, 2], [1, 2, 3])).toBe(false);
    expect(deepEqual([1, 2], [2, 1])).toBe(false);
  });

  it("compares nested structures", () => {
    expect(deepEqual({ a: [1, { b: 2 }] }, { a: [1, { b: 2 }] })).toBe(true);
    expect(deepEqual({ a: [1, { b: 2 }] }, { a: [1, { b: 3 }] })).toBe(false);
  });

  it("ignores key order", () => {
    expect(deepEqual({ a: 1, b: 2 }, { b: 2, a: 1 })).toBe(true);
  });

  it("counts the keys on both sides", () => {
    // Comparing only a's keys would call these equal.
    expect(deepEqual({ a: 1 }, { a: 1, b: 2 })).toBe(false);
    expect(deepEqual({ a: 1, b: 2 }, { a: 1 })).toBe(false);
  });

  it("tells a missing key from an undefined one", () => {
    expect(deepEqual({ a: undefined }, {})).toBe(false);
  });

  it("does not confuse an array with an object", () => {
    expect(deepEqual([], {})).toBe(false);
    expect(deepEqual([1], { 0: 1 })).toBe(false);
  });

  it("compares empty structures", () => {
    expect(deepEqual({}, {})).toBe(true);
    expect(deepEqual([], [])).toBe(true);
  });
});

describe("ex056 deepEqual — dates", () => {
  it("compares by time, not identity", () => {
    expect(deepEqual(new Date("2024-01-31"), new Date("2024-01-31"))).toBe(true);
    expect(deepEqual(new Date("2024-01-31"), new Date("2024-02-01"))).toBe(false);
  });

  it("does not call a Date equal to a plain object", () => {
    // Both have zero own enumerable keys, so a naive key comparison says yes.
    expect(deepEqual(new Date("2024-01-31"), {})).toBe(false);
    expect(deepEqual({}, new Date("2024-01-31"))).toBe(false);
  });
});

describe("ex056 deepEqual — cycles", () => {
  it("survives a self-reference", () => {
    const a = { name: "x" };
    a.self = a;
    const b = { name: "x" };
    b.self = b;
    expect(deepEqual(a, b)).toBe(true);
  });

  it("still reports a real difference behind a cycle", () => {
    const a = { name: "x" };
    a.self = a;
    const b = { name: "y" };
    b.self = b;
    expect(deepEqual(a, b)).toBe(false);
  });

  it("survives a mutual cycle between two objects", () => {
    const makePair = () => {
      const left = { tag: "left" };
      const right = { tag: "right", left };
      left.right = right;
      return left;
    };
    expect(deepEqual(makePair(), makePair())).toBe(true);
  });
});

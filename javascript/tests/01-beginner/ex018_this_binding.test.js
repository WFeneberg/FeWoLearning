import { describe, expect, it } from "vitest";
import {
  bindMethod,
  borrowSlice,
  invokeAs,
  thisWhenCalledBare,
} from "@ex/01-beginner/ex018_this_binding/index.js";

const counter = () => ({
  count: 0,
  add(amount) {
    this.count += amount;
    return this.count;
  },
});

describe("ex018 invokeAs", () => {
  it("points a function at a receiver", () => {
    const target = counter();
    expect(invokeAs(target.add, target, [5])).toBe(5);
    expect(target.count).toBe(5);
  });

  it("can aim the same function at a different object", () => {
    const source = counter();
    const other = { count: 100 };
    invokeAs(source.add, other, [1]);
    expect(other.count).toBe(101);
    expect(source.count).toBe(0);
  });

  it("passes no arguments as an empty list", () => {
    expect(invokeAs(function () { return this.tag; }, { tag: "x" }, [])).toBe("x");
  });
});

describe("ex018 bindMethod", () => {
  it("survives detachment", () => {
    const target = counter();
    const add = bindMethod(target, "add");
    expect(add(2)).toBe(2);
    expect(add(3)).toBe(5);
    expect(target.count).toBe(5);
  });

  it("cannot be re-aimed — a bound function ignores call()", () => {
    const target = counter();
    const add = bindMethod(target, "add");
    add.call({ count: 999 }, 1);
    expect(target.count).toBe(1);
  });

  it("does not replace the method on the object", () => {
    const target = counter();
    const before = target.add;
    bindMethod(target, "add");
    expect(target.add).toBe(before);
  });
});

describe("ex018 thisWhenCalledBare", () => {
  it("is undefined, because a module is strict-mode code", () => {
    // In sloppy mode this would be globalThis. Nothing here is sloppy mode.
    expect(thisWhenCalledBare()).toBeUndefined();
  });
});

describe("ex018 borrowSlice", () => {
  it("turns an array-like into a real array", () => {
    const result = borrowSlice({ 0: "a", 1: "b", length: 2 });
    expect(Array.isArray(result)).toBe(true);
    expect(result).toEqual(["a", "b"]);
  });

  it("stops at length, ignoring the extra keys", () => {
    expect(borrowSlice({ 0: "a", 1: "b", 2: "c", length: 2 })).toEqual(["a", "b"]);
  });

  it("gives an empty array for length 0", () => {
    expect(borrowSlice({ length: 0 })).toEqual([]);
  });
});

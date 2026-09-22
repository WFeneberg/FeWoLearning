import { describe, expect, it } from "vitest";
import { at, firstOr, sumAt } from "@ex/01-beginner/ex007_safe_indexing/index";

describe("ex007 at", () => {
  it("reads an element in range", () => {
    expect(at(["a", "b"], 1)).toBe("b");
  });

  it("returns undefined past the end rather than throwing", () => {
    expect(at(["a", "b"], 7)).toBeUndefined();
  });

  it("returns undefined for a negative index", () => {
    expect(at(["a"], -1)).toBeUndefined();
  });
});

describe("ex007 firstOr", () => {
  it("returns the first element when there is one", () => {
    expect(firstOr(["x", "y"], "none")).toBe("x");
  });

  it("falls back on an empty list", () => {
    expect(firstOr([], "none")).toBe("none");
  });
});

describe("ex007 sumAt", () => {
  it("adds the values at the given indices", () => {
    expect(sumAt([10, 20, 30], [0, 2])).toBe(40);
  });

  it("skips indices with no value", () => {
    expect(sumAt([10, 20], [0, 5, 1])).toBe(30);
  });
});

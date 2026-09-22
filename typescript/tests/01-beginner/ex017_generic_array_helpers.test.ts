import { describe, expect, it } from "vitest";
import { chunk, first, last } from "@ex/01-beginner/ex017_generic_array_helpers/index";

describe("ex017 first and last", () => {
  it("read the ends of a list", () => {
    expect(first(["a", "b", "c"])).toBe("a");
    expect(last(["a", "b", "c"])).toBe("c");
  });

  it("return undefined for an empty list", () => {
    expect(first([])).toBeUndefined();
    expect(last([])).toBeUndefined();
  });

  it("agree on a one-element list", () => {
    expect(first([7])).toBe(7);
    expect(last([7])).toBe(7);
  });
});

describe("ex017 chunk", () => {
  it("splits into full runs", () => {
    expect(chunk([1, 2, 3, 4], 2)).toEqual([
      [1, 2],
      [3, 4],
    ]);
  });

  it("leaves a short final run", () => {
    expect(chunk([1, 2, 3], 2)).toEqual([[1, 2], [3]]);
  });

  it("returns an empty result for an empty input", () => {
    expect(chunk([], 3)).toEqual([]);
  });

  it("keeps order", () => {
    expect(chunk(["a", "b", "c", "d", "e"], 3)).toEqual([
      ["a", "b", "c"],
      ["d", "e"],
    ]);
  });
});

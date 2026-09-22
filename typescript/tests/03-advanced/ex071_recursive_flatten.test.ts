import { describe, expect, it } from "vitest";
import { flattenDeep } from "@ex/03-advanced/ex071_recursive_flatten/index";

describe("ex071 flattenDeep", () => {
  it("flattens one level", () => {
    expect(flattenDeep([[1, 2], [3]])).toEqual([1, 2, 3]);
  });

  it("flattens all the way down", () => {
    expect(flattenDeep([1, [2, [3, [4]]]])).toEqual([1, 2, 3, 4]);
  });

  it("keeps order", () => {
    expect(flattenDeep([[["a"], "b"], "c"])).toEqual(["a", "b", "c"]);
  });

  it("handles an already-flat list and an empty one", () => {
    expect(flattenDeep([1, 2])).toEqual([1, 2]);
    expect(flattenDeep([])).toEqual([]);
  });

  it("drops empty nested arrays rather than keeping holes", () => {
    expect(flattenDeep([1, [], [2, []]])).toEqual([1, 2]);
  });
});

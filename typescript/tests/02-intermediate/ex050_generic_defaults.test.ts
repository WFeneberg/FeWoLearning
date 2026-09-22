import { describe, expect, it } from "vitest";
import { makeList } from "@ex/02-intermediate/ex050_generic_defaults/index";

describe("ex050 makeList", () => {
  it("collects its arguments", () => {
    expect(makeList(1, 2, 3)).toEqual([1, 2, 3]);
  });

  it("returns an empty list when given nothing", () => {
    expect(makeList()).toEqual([]);
  });

  it("returns a new array rather than the arguments object", () => {
    const items = ["a", "b"];
    const result = makeList(...items);
    expect(result).toEqual(["a", "b"]);
    expect(result).not.toBe(items);
  });
});

import { describe, expect, it } from "vitest";
import { fromKeys } from "@ex/02-intermediate/ex043_rebuild_record/index";

describe("ex043 fromKeys", () => {
  it("maps every key to the value", () => {
    expect(fromKeys(["a", "b"], 0)).toEqual({ a: 0, b: 0 });
  });

  it("returns an empty object for no keys", () => {
    expect(fromKeys([], 1)).toEqual({});
  });

  it("collapses a repeated key", () => {
    expect(fromKeys(["a", "a"], 3)).toEqual({ a: 3 });
  });
});

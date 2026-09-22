import { describe, expect, it } from "vitest";
import { getIn, pluck, setIn } from "@ex/02-intermediate/ex049_constrained_key_generics/index";

const row = { id: "r-1", label: "first", count: 2 };

describe("ex049 getIn", () => {
  it("reads a property", () => {
    expect(getIn(row, "label")).toBe("first");
    expect(getIn(row, "count")).toBe(2);
  });
});

describe("ex049 setIn", () => {
  it("replaces one property", () => {
    expect(setIn(row, "count", 9)).toEqual({ id: "r-1", label: "first", count: 9 });
  });

  it("does not mutate the source", () => {
    setIn(row, "count", 9);
    expect(row.count).toBe(2);
  });
});

describe("ex049 pluck", () => {
  it("collects one property from every element", () => {
    const rows = [
      { id: "a", count: 1 },
      { id: "b", count: 2 },
    ];
    expect(pluck(rows, "id")).toEqual(["a", "b"]);
    expect(pluck(rows, "count")).toEqual([1, 2]);
  });

  it("returns an empty list for no elements", () => {
    expect(pluck([] as { id: string }[], "id")).toEqual([]);
  });
});

import { describe, expect, it } from "vitest";
import { omitAt } from "@ex/02-intermediate/ex042_rebuild_pick_omit/index";
import type { Row } from "@ex/02-intermediate/ex042_rebuild_pick_omit/index";

function makeRow(): Row {
  return { id: "r-1", label: "first", count: 2 };
}

describe("ex042 omitAt", () => {
  it("removes one key", () => {
    expect(omitAt(makeRow(), ["label"])).toEqual({ id: "r-1", count: 2 });
  });

  it("removes several", () => {
    expect(omitAt(makeRow(), ["label", "count"])).toEqual({ id: "r-1" });
  });

  it("removes nothing when given no keys", () => {
    expect(omitAt(makeRow(), [])).toEqual({ id: "r-1", label: "first", count: 2 });
  });

  it("does not mutate the source", () => {
    const row = makeRow();
    omitAt(row, ["label"]);
    expect(row).toEqual({ id: "r-1", label: "first", count: 2 });
  });
});

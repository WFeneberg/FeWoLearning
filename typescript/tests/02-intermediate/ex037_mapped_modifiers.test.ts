import { describe, expect, it } from "vitest";
import { solidify } from "@ex/02-intermediate/ex037_mapped_modifiers/index";

describe("ex037 solidify", () => {
  it("keeps a label that is there", () => {
    expect(solidify({ id: "r-1", label: "first", count: 2 })).toEqual({
      id: "r-1",
      label: "first",
      count: 2,
    });
  });

  it("supplies an empty label when there is none", () => {
    expect(solidify({ id: "r-1", count: 2 })).toEqual({
      id: "r-1",
      label: "",
      count: 2,
    });
  });
});

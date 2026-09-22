import { describe, expect, it } from "vitest";
import { stringifyAll } from "@ex/02-intermediate/ex036_mapped_types/index";

describe("ex036 stringifyAll", () => {
  it("renders every field as a string", () => {
    expect(stringifyAll({ id: "u-1", age: 41, active: true })).toEqual({
      id: "u-1",
      age: "41",
      active: "true",
    });
  });

  it("renders falsy values rather than dropping them", () => {
    expect(stringifyAll({ id: "", age: 0, active: false })).toEqual({
      id: "",
      age: "0",
      active: "false",
    });
  });
});

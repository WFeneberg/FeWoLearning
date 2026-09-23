import { describe, expect, it } from "vitest";
import { asUserId, describeUser } from "@ex/03-advanced/ex076_branded_types/index";

describe("ex076 asUserId", () => {
  // The brand is phantom: nothing is added at runtime.
  it("gives back the very same string", () => {
    expect(asUserId("U-1")).toBe("U-1");
  });

  // Object.keys of a branded string gives ['0','1','2'] — its character
  // indices — because a branded string IS a string, which is the point.
  // The brand itself leaves no trace at all.
  it("is still a string, with no brand anywhere in it", () => {
    expect(typeof asUserId("U-1")).toBe("string");
    expect(JSON.stringify({ id: asUserId("U-1") })).toBe('{"id":"U-1"}');
  });
});

describe("ex076 describeUser", () => {
  it("uses the value as the string it still is", () => {
    expect(describeUser(asUserId("U-1"))).toBe("user:u-1");
  });
});

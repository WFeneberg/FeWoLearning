import { describe, expect, it } from "vitest";
import { describe as describeInput } from "@ex/01-beginner/ex004_union_narrowing/index";

describe("ex004 describe", () => {
  it("tags a string", () => {
    expect(describeInput("hello")).toBe("text:hello");
  });

  it("tags a number", () => {
    expect(describeInput(42)).toBe("number:42");
  });

  it("tags a boolean", () => {
    expect(describeInput(true)).toBe("boolean:true");
    expect(describeInput(false)).toBe("boolean:false");
  });

  it("reports an array by length, not by typeof", () => {
    // typeof [] is "object", so a typeof-only chain never gets here.
    expect(describeInput(["a", "b", "c"])).toBe("list:3");
  });

  it("handles the empty array", () => {
    expect(describeInput([])).toBe("list:0");
  });
});

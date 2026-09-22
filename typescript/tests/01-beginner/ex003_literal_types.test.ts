import { describe, expect, it } from "vitest";
import { LEVELS, isAtLeast } from "@ex/01-beginner/ex003_literal_types/index";

describe("ex003 LEVELS", () => {
  it("lists the four levels in ascending severity", () => {
    expect(LEVELS).toEqual(["debug", "info", "warn", "error"]);
  });
});

describe("ex003 isAtLeast", () => {
  it("is true for a more severe level", () => {
    expect(isAtLeast("warn", "info")).toBe(true);
  });

  it("is true for the same level", () => {
    expect(isAtLeast("info", "info")).toBe(true);
  });

  it("is false for a less severe level", () => {
    expect(isAtLeast("debug", "info")).toBe(false);
  });

  it("orders the endpoints correctly", () => {
    expect(isAtLeast("error", "debug")).toBe(true);
    expect(isAtLeast("debug", "error")).toBe(false);
  });
});

import { describe, expect, it } from "vitest";
import { identity, longer } from "@ex/01-beginner/ex016_generics_and_constraints/index";

describe("ex016 identity", () => {
  it("gives back what it was handed", () => {
    expect(identity(42)).toBe(42);
  });

  it("gives back the same reference, not a copy", () => {
    const value = { a: 1 };
    expect(identity(value)).toBe(value);
  });
});

describe("ex016 longer", () => {
  it("picks the longer string", () => {
    expect(longer("ab", "c")).toBe("ab");
    expect(longer("a", "bcd")).toBe("bcd");
  });

  it("picks the longer array", () => {
    expect(longer([1, 2, 3], [1])).toEqual([1, 2, 3]);
  });

  it("returns the first argument on a tie", () => {
    expect(longer("ab", "cd")).toBe("ab");
  });
});

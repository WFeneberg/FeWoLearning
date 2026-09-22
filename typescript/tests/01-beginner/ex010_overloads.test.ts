import { describe, expect, it } from "vitest";
import { parse } from "@ex/01-beginner/ex010_overloads/index";

describe("ex010 parse", () => {
  it("splits a string on commas", () => {
    expect(parse("a,b,c")).toEqual(["a", "b", "c"]);
  });

  it("returns a single-element list for a string without commas", () => {
    expect(parse("solo")).toEqual(["solo"]);
  });

  it("splits a number into its digits", () => {
    expect(parse(407)).toEqual([4, 0, 7]);
  });

  it("handles a single-digit number", () => {
    expect(parse(7)).toEqual([7]);
  });
});

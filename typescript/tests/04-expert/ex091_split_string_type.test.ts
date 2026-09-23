import { describe, expect, it } from "vitest";
import { split } from "@ex/04-expert/ex091_split_string_type/index";

describe("ex091 split", () => {
  it("splits on a single-character delimiter", () => {
    expect(split("a,b,c", ",")).toEqual(["a", "b", "c"]);
  });

  it("gives back the whole string when the delimiter is absent", () => {
    expect(split("abc", ",")).toEqual(["abc"]);
  });

  // The base case: an empty string is one empty piece, not none.
  it("splits an empty string into one empty piece", () => {
    expect(split("", ",")).toEqual([""]);
  });

  it("keeps the empty pieces between adjacent delimiters", () => {
    expect(split("a,,b", ",")).toEqual(["a", "", "b"]);
  });

  it("keeps a trailing empty piece", () => {
    expect(split("a,", ",")).toEqual(["a", ""]);
  });

  it("handles a multi-character delimiter", () => {
    expect(split("a::b::c", "::")).toEqual(["a", "b", "c"]);
  });
});

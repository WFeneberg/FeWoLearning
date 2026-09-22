import { describe, expect, it } from "vitest";
import { countWords, readFrom } from "@ex/02-intermediate/ex067_index_signatures/index";

describe("ex067 countWords", () => {
  it("counts each word", () => {
    expect(countWords("a b a")).toEqual({ a: 2, b: 1 });
  });

  it("ignores runs of whitespace", () => {
    expect(countWords("  a   b  ")).toEqual({ a: 1, b: 1 });
  });

  it("returns an empty bag for empty text", () => {
    expect(countWords("")).toEqual({});
    expect(countWords("   ")).toEqual({});
  });
});

describe("ex067 readFrom", () => {
  it("reads a key that is there", () => {
    expect(readFrom({ a: 1 }, "a")).toBe(1);
  });

  it("gives undefined for one that is not", () => {
    expect(readFrom({ a: 1 }, "zz")).toBeUndefined();
  });
});

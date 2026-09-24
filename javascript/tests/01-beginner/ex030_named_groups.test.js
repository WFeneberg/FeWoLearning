import { describe, expect, it } from "vitest";
import {
  parseLogLine,
  parsePairs,
  reformatDates,
  staleLastIndex,
} from "@ex/01-beginner/ex030_named_groups/index.js";

describe("ex030 parseLogLine", () => {
  it("names the fields", () => {
    expect(parseLogLine("2024-01-31 ERROR disk full")).toEqual({
      date: "2024-01-31",
      level: "ERROR",
      message: "disk full",
    });
  });

  it("keeps the whole message, spaces and all", () => {
    expect(parseLogLine("2024-01-31 WARN a b c").message).toBe("a b c");
  });

  it("returns null for a line that does not match", () => {
    expect(parseLogLine("nonsense")).toBeNull();
    expect(parseLogLine("2024-01-31 error lowercase level")).toBeNull();
  });
});

describe("ex030 parsePairs", () => {
  it("finds every pair, in order", () => {
    expect(parsePairs("a=1 b=2")).toEqual([
      { key: "a", value: "1" },
      { key: "b", value: "2" },
    ]);
  });

  it("copes with commas and repeated keys", () => {
    expect(parsePairs("x=1, x=2")).toEqual([
      { key: "x", value: "1" },
      { key: "x", value: "2" },
    ]);
  });

  it("returns an empty array when there is nothing to find", () => {
    expect(parsePairs("no pairs")).toEqual([]);
  });

  it("can be called twice with the same result", () => {
    // matchAll starts from lastIndex 0 every time it is called on a string,
    // so a repeated call cannot come back short the way test() does.
    expect(parsePairs("a=1")).toEqual(parsePairs("a=1"));
  });
});

describe("ex030 reformatDates", () => {
  it("rewrites by name", () => {
    expect(reformatDates("due 31.01.2024")).toBe("due 2024-01-31");
  });

  it("rewrites every occurrence", () => {
    expect(reformatDates("01.02.2024 and 03.04.2025")).toBe("2024-02-01 and 2025-04-03");
  });

  it("leaves text without dates alone", () => {
    expect(reformatDates("nothing here")).toBe("nothing here");
  });
});

describe("ex030 staleLastIndex", () => {
  it("answers true then false for the same input", () => {
    expect(staleLastIndex()).toEqual([true, false]);
  });
});

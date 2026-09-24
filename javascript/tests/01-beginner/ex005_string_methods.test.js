import { describe, expect, it } from "vitest";
import {
  lastChar,
  maskTail,
  mutateFirstChar,
  normalizeSpaces,
  padId,
} from "@ex/01-beginner/ex005_string_methods/index.js";

describe("ex005 normalizeSpaces", () => {
  it("trims the ends and collapses the middle", () => {
    expect(normalizeSpaces("  Ada   Lovelace \n")).toBe("Ada Lovelace");
    expect(normalizeSpaces("\tone\t\ttwo\n\nthree ")).toBe("one two three");
  });

  it("handles the degenerate inputs", () => {
    expect(normalizeSpaces("")).toBe("");
    expect(normalizeSpaces("   ")).toBe("");
    expect(normalizeSpaces("solo")).toBe("solo");
  });
});

describe("ex005 padId", () => {
  it("pads to the requested width", () => {
    expect(padId(42, 5)).toBe("00042");
    expect(padId(0, 3)).toBe("000");
  });

  it("never truncates an over-long value", () => {
    expect(padId(123456, 3)).toBe("123456");
  });
});

describe("ex005 maskTail", () => {
  it("replaces every occurrence, not just the first", () => {
    expect(maskTail("a-b-c", "-", "_")).toBe("a_b_c");
    expect("a-b-c".replace("-", "_")).toBe("a_b-c");
  });

  it("returns the input unchanged when there is no match", () => {
    expect(maskTail("abc", "-", "_")).toBe("abc");
  });

  it("can remove rather than replace", () => {
    expect(maskTail("1 000 000", " ", "")).toBe("1000000");
  });
});

describe("ex005 lastChar", () => {
  it("reads from the end", () => {
    expect(lastChar("abc")).toBe("c");
    expect(lastChar("x")).toBe("x");
  });

  it("returns undefined for the empty string", () => {
    expect(lastChar("")).toBeUndefined();
  });
});

describe("ex005 mutateFirstChar", () => {
  it("cannot change the string, and says so by returning the original", () => {
    expect(mutateFirstChar("abc")).toBe("abc");
  });

  it("does not let the TypeError escape", () => {
    // Strict-mode assignment to a string index throws; the exercise is to
    // survive that, not to propagate it.
    expect(() => mutateFirstChar("hello")).not.toThrow();
    expect(mutateFirstChar("hello")).toBe("hello");
  });

  it("leaves the caller's variable pointing at the same value", () => {
    const original = "immutable";
    mutateFirstChar(original);
    expect(original).toBe("immutable");
  });
});

import { describe, expect, it } from "vitest";
import {
  countLetters,
  interpolate,
  pricesOnly,
  tokenize,
  wordsNotAfterNo,
} from "@ex/03-advanced/ex090_regex_advanced/index.js";

describe("ex090 pricesOnly", () => {
  it("keeps only the numbers behind a currency symbol", () => {
    expect(pricesOnly("$12 and 34 and €5")).toEqual(["12", "5"]);
  });

  it("does not consume the symbol — a lookbehind is zero-width", () => {
    expect(pricesOnly("$12")).toEqual(["12"]);
  });

  it("is empty when nothing matches", () => {
    expect(pricesOnly("no prices here")).toEqual([]);
    expect(pricesOnly("")).toEqual([]);
  });
});

describe("ex090 wordsNotAfterNo", () => {
  it("excludes the word right after 'no'", () => {
    expect(wordsNotAfterNo("no cats and dogs")).toEqual(["no", "and", "dogs"]);
  });

  it("excludes nothing when 'no' does not occur", () => {
    expect(wordsNotAfterNo("cats and dogs")).toEqual(["cats", "and", "dogs"]);
  });
});

describe("ex090 tokenize", () => {
  it("splits an expression", () => {
    expect(tokenize("12+7-3")).toEqual([
      { type: "number", value: "12" },
      { type: "operator", value: "+" },
      { type: "number", value: "7" },
      { type: "operator", value: "-" },
      { type: "number", value: "3" },
    ]);
  });

  it("refuses to skip over anything", () => {
    // A /g pattern would simply find the tokens either side and lose the
    // "?"; a sticky one stops dead, which is what a lexer needs.
    expect(() => tokenize("1?2")).toThrow(SyntaxError);
    expect(() => tokenize("1?2")).toThrow("unexpected character at 1");
  });

  it("reports the position of the first bad character", () => {
    expect(() => tokenize("12+x")).toThrow("unexpected character at 3");
  });

  it("returns nothing for an empty string", () => {
    expect(tokenize("")).toEqual([]);
  });

  it("does not leak lastIndex between calls", () => {
    expect(tokenize("1+2")).toEqual(tokenize("1+2"));
  });
});

describe("ex090 countLetters", () => {
  it("counts letters of any script", () => {
    expect(countLetters("Grüße 日本語 123")).toBe(8);
  });

  it("counts nothing but letters", () => {
    expect(countLetters("123 !@# \n")).toBe(0);
    expect(countLetters("")).toBe(0);
  });

  it("counts an accented letter once", () => {
    expect(countLetters("é")).toBe(1);
  });
});

describe("ex090 interpolate", () => {
  it("fills the placeholders", () => {
    expect(interpolate("{greeting}, {name}!", { greeting: "Hi", name: "Ada" })).toBe("Hi, Ada!");
  });

  it("leaves an unknown placeholder alone", () => {
    expect(interpolate("{a} and {missing}", { a: 1 })).toBe("1 and {missing}");
  });

  it("stringifies non-string values", () => {
    expect(interpolate("{n}", { n: 0 })).toBe("0");
    expect(interpolate("{b}", { b: false })).toBe("false");
  });

  it("does not read inherited keys", () => {
    expect(interpolate("{toString}", {})).toBe("{toString}");
  });

  it("leaves text with no placeholders untouched", () => {
    expect(interpolate("plain", {})).toBe("plain");
  });
});

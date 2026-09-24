import { describe, expect, it } from "vitest";
import {
  countWord,
  firstNumber,
  isHexColor,
  maskDigits,
} from "@ex/01-beginner/ex029_regex_basics/index.js";

describe("ex029 isHexColor", () => {
  it("accepts both lengths, either case", () => {
    expect(isHexColor("#fff")).toBe(true);
    expect(isHexColor("#a1b2c3")).toBe(true);
    expect(isHexColor("#ABCDEF")).toBe(true);
  });

  it("is anchored at both ends", () => {
    // Without ^ and $ every one of these would match somewhere inside.
    expect(isHexColor("#fff and more")).toBe(false);
    expect(isHexColor("colour: #fff")).toBe(false);
    expect(isHexColor("#fffff")).toBe(false);
  });

  it("rejects the near misses", () => {
    expect(isHexColor("fff")).toBe(false);
    expect(isHexColor("#ggg")).toBe(false);
    expect(isHexColor("")).toBe(false);
  });
});

describe("ex029 firstNumber", () => {
  it("returns the first run of digits as a number", () => {
    expect(firstNumber("order 42 of 99")).toBe(42);
    expect(firstNumber("7 items")).toBe(7);
  });

  it("returns null rather than NaN when there is none", () => {
    expect(firstNumber("no digits here")).toBeNull();
    expect(firstNumber("")).toBeNull();
  });

  it("keeps the whole run together", () => {
    expect(firstNumber("id 1234x")).toBe(1234);
  });
});

describe("ex029 maskDigits", () => {
  it("replaces every digit", () => {
    expect(maskDigits("a1b2c3")).toBe("axbxcx");
    // Without /g only the first one would go.
    expect("a1b2".replace(/\d/, "x")).toBe("axb2");
  });

  it("leaves text without digits alone", () => {
    expect(maskDigits("abc")).toBe("abc");
  });
});

describe("ex029 countWord", () => {
  it("counts whole words, ignoring case", () => {
    expect(countWord("Cat cats CAT", "cat")).toBe(2);
  });

  it("is 0 when the word does not occur", () => {
    expect(countWord("dogs", "cat")).toBe(0);
  });

  it("builds its pattern from the runtime argument", () => {
    const word = ["ho", "use"].join("");
    expect(countWord("a house, the house", word)).toBe(2);
  });

  it("does not count a substring of a longer word", () => {
    expect(countWord("category catalogue", "cat")).toBe(0);
  });
});

import { describe, expect, it } from "vitest";
import {
  decodeUtf8,
  encodeUtf8,
  measure,
  truncateBytes,
} from "@ex/02-intermediate/ex068_text_encoding/index.js";

describe("ex068 encodeUtf8 / decodeUtf8", () => {
  it("encodes ASCII one byte per character", () => {
    const bytes = encodeUtf8("abc");
    expect(bytes).toBeInstanceOf(Uint8Array);
    expect([...bytes]).toEqual([97, 98, 99]);
  });

  it("encodes non-ASCII in more than one byte", () => {
    expect([...encodeUtf8("é")]).toEqual([195, 169]);
    expect(encodeUtf8("😀")).toHaveLength(4);
  });

  it("round-trips", () => {
    for (const text of ["", "plain", "Grüße", "日本語", "a😀b"]) {
      expect(decodeUtf8(encodeUtf8(text))).toBe(text);
    }
  });
});

describe("ex068 measure", () => {
  it("gives three different numbers for one emoji", () => {
    expect(measure("😀")).toEqual({ codeUnits: 2, codePoints: 1, bytes: 4 });
  });

  it("agrees on plain ASCII", () => {
    expect(measure("abc")).toEqual({ codeUnits: 3, codePoints: 3, bytes: 3 });
  });

  it("separates code units from bytes for Latin-1 accents", () => {
    expect(measure("é")).toEqual({ codeUnits: 1, codePoints: 1, bytes: 2 });
  });

  it("handles the empty string", () => {
    expect(measure("")).toEqual({ codeUnits: 0, codePoints: 0, bytes: 0 });
  });
});

describe("ex068 truncateBytes", () => {
  it("cuts ASCII at the byte count", () => {
    expect(truncateBytes("abcdef", 3)).toBe("abc");
  });

  it("never splits a multi-byte character", () => {
    // "😀" is 4 bytes; a budget of 3 can hold none of it.
    expect(truncateBytes("😀", 3)).toBe("");
    expect(truncateBytes("😀", 4)).toBe("😀");
    expect(truncateBytes("a😀", 4)).toBe("a");
    expect(truncateBytes("a😀", 5)).toBe("a😀");
  });

  it("always produces decodable output", () => {
    for (let budget = 0; budget <= 12; budget++) {
      const cut = truncateBytes("aé😀b", budget);
      expect(decodeUtf8(encodeUtf8(cut))).toBe(cut);
      expect("aé😀b".startsWith(cut)).toBe(true);
    }
  });

  it("returns the whole string when the budget is generous", () => {
    expect(truncateBytes("abc", 100)).toBe("abc");
  });
});

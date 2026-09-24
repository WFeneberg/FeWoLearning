import { describe, expect, it } from "vitest";
import {
  decodeRecord,
  encodedLength,
  encodeRecord,
} from "@ex/03-advanced/ex089_binary_records/index.js";

const record = () => ({ version: 3, name: "sensor-a", tags: ["temp", "indoor"] });

describe("ex089 encodedLength", () => {
  it("counts the headers and the bytes", () => {
    // 2 version + 1 nameLen + 8 name + 2 tagCount + (1+4) + (1+6) = 25
    expect(encodedLength(record())).toBe(25);
  });

  it("counts UTF-8 bytes rather than characters", () => {
    expect(encodedLength({ version: 1, name: "é", tags: [] })).toBe(2 + 1 + 2 + 2);
  });

  it("matches what encodeRecord actually produces", () => {
    for (const value of [record(), { version: 0, name: "", tags: [] }]) {
      expect(encodeRecord(value).byteLength).toBe(encodedLength(value));
    }
  });
});

describe("ex089 encodeRecord", () => {
  it("writes the header big-endian", () => {
    const bytes = new Uint8Array(encodeRecord({ version: 258, name: "", tags: [] }));
    expect([...bytes.slice(0, 3)]).toEqual([1, 2, 0]);
  });

  it("returns an ArrayBuffer", () => {
    expect(encodeRecord(record())).toBeInstanceOf(ArrayBuffer);
  });
});

describe("ex089 decodeRecord", () => {
  it("round-trips", () => {
    expect(decodeRecord(encodeRecord(record()))).toEqual(record());
  });

  it("round-trips the empty cases", () => {
    const empty = { version: 0, name: "", tags: [] };
    expect(decodeRecord(encodeRecord(empty))).toEqual(empty);
  });

  it("round-trips non-ASCII", () => {
    const value = { version: 1, name: "Grüße 😀", tags: ["außen", "日本語"] };
    expect(decodeRecord(encodeRecord(value))).toEqual(value);
  });

  it("round-trips the maximum lengths the format allows", () => {
    const value = { version: 65_535, name: "x".repeat(255), tags: ["y".repeat(255)] };
    expect(decodeRecord(encodeRecord(value))).toEqual(value);
  });

  it("throws a RangeError on a truncated buffer", () => {
    // Not "returns whatever fits": a short read must be loud.
    const full = encodeRecord(record());
    expect(() => decodeRecord(full.slice(0, 10))).toThrow(RangeError);
    expect(() => decodeRecord(full.slice(0, 2))).toThrow(RangeError);
  });

  it("reads several records written with the same helpers", () => {
    const values = [record(), { version: 9, name: "b", tags: [] }];
    expect(values.map((value) => decodeRecord(encodeRecord(value)))).toEqual(values);
  });
});

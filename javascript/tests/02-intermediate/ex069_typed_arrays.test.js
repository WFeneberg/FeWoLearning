import { describe, expect, it } from "vitest";
import {
  endiannessBytes,
  packRecord,
  sharedBuffer,
  unpackRecord,
} from "@ex/02-intermediate/ex069_typed_arrays/index.js";

describe("ex069 packRecord", () => {
  it("produces a 12-byte buffer", () => {
    const buffer = packRecord(1, 2.5);
    expect(buffer).toBeInstanceOf(ArrayBuffer);
    expect(buffer.byteLength).toBe(12);
  });

  it("writes the id big-endian", () => {
    // Big-endian means the most significant byte first — 0x00 00 00 01.
    expect([...new Uint8Array(packRecord(1, 0)).slice(0, 4)]).toEqual([0, 0, 0, 1]);
    expect([...new Uint8Array(packRecord(0x01020304, 0)).slice(0, 4)]).toEqual([1, 2, 3, 4]);
  });

  it("round-trips", () => {
    expect(unpackRecord(packRecord(42, 3.5))).toEqual({ id: 42, score: 3.5 });
    expect(unpackRecord(packRecord(0, -0.125))).toEqual({ id: 0, score: -0.125 });
    expect(unpackRecord(packRecord(4_294_967_295, 0))).toEqual({ id: 4_294_967_295, score: 0 });
  });

  it("keeps float precision that a 32-bit float would lose", () => {
    expect(unpackRecord(packRecord(1, 0.1)).score).toBe(0.1);
  });
});

describe("ex069 endiannessBytes", () => {
  it("writes the same number in both byte orders", () => {
    expect(endiannessBytes()).toEqual([1, 2, 3, 4, 4, 3, 2, 1]);
  });
});

describe("ex069 sharedBuffer", () => {
  it("shows one write through two views", () => {
    // Byte 0 is the LOW byte on a little-endian machine, so 255 in byte 0
    // reads as 255 — not as 0xFF000000.
    expect(sharedBuffer()).toEqual({ bytes: [255, 0, 0, 0], asUint32: 255 });
  });
});

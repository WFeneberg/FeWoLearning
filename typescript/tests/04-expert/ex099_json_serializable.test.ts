import { describe, expect, it } from "vitest";
import { roundTrip } from "@ex/04-expert/ex099_json_serializable/index";

describe("ex099 roundTrip", () => {
  it("keeps the plain values", () => {
    expect(roundTrip({ a: 1, b: "x", c: true, d: null })).toEqual({
      a: 1,
      b: "x",
      c: true,
      d: null,
    });
  });

  it("drops a function-valued property", () => {
    expect(roundTrip({ a: 1, run: () => 1 })).toEqual({ a: 1 });
  });

  it("drops an undefined-valued property", () => {
    expect(roundTrip({ a: 1, b: undefined })).toEqual({ a: 1 });
  });

  // The one that bites: a Date comes back as a string.
  it("turns a Date into a string", () => {
    const value = roundTrip({ at: new Date("2026-01-01T00:00:00.000Z") });
    expect(value).toEqual({ at: "2026-01-01T00:00:00.000Z" });
    expect(typeof (value as { at: unknown }).at).toBe("string");
  });

  it("empties a Map and a Set", () => {
    expect(roundTrip({ m: new Map([["a", 1]]), s: new Set([1]) })).toEqual({ m: {}, s: {} });
  });

  it("recurses into arrays and nested objects", () => {
    expect(roundTrip({ rows: [{ a: 1, run: () => 1 }] })).toEqual({ rows: [{ a: 1 }] });
  });
});

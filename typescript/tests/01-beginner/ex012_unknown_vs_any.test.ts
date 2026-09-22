import { describe, expect, it } from "vitest";
import { asRecord, lengthOf, parseJson } from "@ex/01-beginner/ex012_unknown_vs_any/index";

describe("ex012 parseJson", () => {
  it("parses an object", () => {
    expect(parseJson('{"a":1}')).toEqual({ a: 1 });
  });

  it("parses a bare value", () => {
    expect(parseJson("42")).toBe(42);
  });
});

describe("ex012 lengthOf", () => {
  it("measures a string", () => {
    expect(lengthOf("hello")).toBe(5);
  });

  it("measures an array", () => {
    expect(lengthOf([1, 2, 3])).toBe(3);
  });

  it("reports 0 for anything else", () => {
    expect(lengthOf(42)).toBe(0);
    expect(lengthOf(null)).toBe(0);
    expect(lengthOf({ length: 9 })).toBe(0);
  });
});

describe("ex012 asRecord", () => {
  it("accepts a plain object", () => {
    expect(asRecord({ a: 1 })).toEqual({ a: 1 });
  });

  it("rejects null, arrays and primitives", () => {
    expect(asRecord(null)).toBeUndefined();
    expect(asRecord([1, 2])).toBeUndefined();
    expect(asRecord("text")).toBeUndefined();
  });
});

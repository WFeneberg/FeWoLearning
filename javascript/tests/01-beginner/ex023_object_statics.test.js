import { describe, expect, it } from "vitest";
import {
  assignInto,
  invert,
  mapValues,
  pick,
} from "@ex/01-beginner/ex023_object_statics/index.js";

describe("ex023 mapValues", () => {
  it("maps the values and keeps the keys", () => {
    expect(mapValues({ a: 1, b: 2 }, (n) => n * 10)).toEqual({ a: 10, b: 20 });
  });

  it("preserves key order", () => {
    expect(Object.keys(mapValues({ z: 1, a: 2 }, (n) => n))).toEqual(["z", "a"]);
  });

  it("leaves the input alone", () => {
    const input = { a: 1 };
    const result = mapValues(input, (n) => n + 1);
    expect(input).toEqual({ a: 1 });
    expect(result).not.toBe(input);
  });

  it("handles an empty object", () => {
    expect(mapValues({}, (n) => n)).toEqual({});
  });
});

describe("ex023 invert", () => {
  it("swaps", () => {
    expect(invert({ a: "x", b: "y" })).toEqual({ x: "a", y: "b" });
  });

  it("lets the later duplicate win", () => {
    expect(invert({ a: "same", b: "same" })).toEqual({ same: "b" });
  });

  it("stringifies the new keys", () => {
    expect(invert({ a: 1 })).toEqual({ 1: "a" });
  });
});

describe("ex023 pick", () => {
  it("keeps the requested keys", () => {
    expect(pick({ a: 1, b: 2, c: 3 }, ["a", "c"])).toEqual({ a: 1, c: 3 });
  });

  it("skips keys the object does not have", () => {
    expect(pick({ a: 1 }, ["a", "zzz"])).toEqual({ a: 1 });
  });

  it("skips inherited keys — toString is not the object's own", () => {
    const result = pick({ a: 1 }, ["toString", "constructor"]);
    expect(result).toEqual({});
  });

  it("keeps an own key whose value is undefined", () => {
    const result = pick({ a: undefined }, ["a"]);
    expect(Object.hasOwn(result, "a")).toBe(true);
  });
});

describe("ex023 assignInto", () => {
  it("returns the target itself", () => {
    const target = { a: 1 };
    expect(assignInto(target, { b: 2 })).toBe(target);
    expect(target).toEqual({ a: 1, b: 2 });
  });

  it("applies sources left to right", () => {
    expect(assignInto({}, { a: 1 }, { a: 2 }, { a: 3 })).toEqual({ a: 3 });
  });

  it("does not modify the sources", () => {
    const source = { b: 2 };
    assignInto({ a: 1 }, source);
    expect(source).toEqual({ b: 2 });
  });

  it("works with no sources at all", () => {
    const target = { a: 1 };
    expect(assignInto(target)).toBe(target);
  });
});

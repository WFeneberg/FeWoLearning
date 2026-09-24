import { describe, expect, it } from "vitest";
import {
  concatSpreading,
  makeDetachedArrayClass,
  makeTypeGuard,
} from "@ex/03-advanced/ex075_well_known_symbols/index.js";

describe("ex075 makeTypeGuard", () => {
  it("answers instanceof from a predicate", () => {
    const Even = makeTypeGuard((n) => typeof n === "number" && n % 2 === 0);
    expect(4 instanceof Even).toBe(true);
    expect(5 instanceof Even).toBe(false);
  });

  it("works for primitives, which a normal instanceof never matches", () => {
    const Stringy = makeTypeGuard((value) => typeof value === "string");
    expect("hello" instanceof Stringy).toBe(true);
    expect(5 instanceof Stringy).toBe(false);
  });

  it("is not a function and has no prototype to walk", () => {
    const Guard = makeTypeGuard(() => true);
    expect(typeof Guard).toBe("object");
    expect(Guard.prototype).toBeUndefined();
  });

  it("coerces the predicate's answer to a boolean", () => {
    const Truthy = makeTypeGuard((value) => value);
    expect("x" instanceof Truthy).toBe(true);
    expect(0 instanceof Truthy).toBe(false);
  });
});

describe("ex075 makeDetachedArrayClass", () => {
  it("is a real Array subclass", () => {
    const Detached = makeDetachedArrayClass();
    const list = Detached.from([1, 2, 3]);
    expect(list).toBeInstanceOf(Detached);
    expect(list).toBeInstanceOf(Array);
    expect(list.first).toBe(1);
  });

  it("hands back plain Arrays from the derived methods", () => {
    const Detached = makeDetachedArrayClass();
    const list = Detached.from([1, 2, 3]);
    for (const derived of [list.map((n) => n), list.filter(() => true), list.slice(0, 1)]) {
      expect(derived).toBeInstanceOf(Array);
      expect(derived).not.toBeInstanceOf(Detached);
      expect(derived.first).toBeUndefined();
    }
  });

  it("does not do it by overriding the methods", () => {
    // Species is the mechanism; an override would also pass the assertions
    // above, so check that map is still the inherited one.
    const Detached = makeDetachedArrayClass();
    expect(Object.hasOwn(Detached.prototype, "map")).toBe(false);
    expect(Detached.prototype.map).toBe(Array.prototype.map);
  });

  it("contrasts with a subclass that does not set species", () => {
    // Anchored: the contrast alone is a platform fact, true before the
    // exercise is implemented.
    expect(makeDetachedArrayClass().from([1]).map((n) => n)).toBeInstanceOf(Array);
    class Attached extends Array {}
    expect(Attached.from([1]).map((n) => n)).toBeInstanceOf(Attached);
  });
});

describe("ex075 concatSpreading", () => {
  it("spreads an array-like that opts in", () => {
    // concat normally appends a non-array as one element.
    expect(concatSpreading().spreadable).toEqual([1, "a", "b"]);
  });

  it("appends an array that opts out as a single element", () => {
    const { notSpreadable } = concatSpreading();
    expect(notSpreadable).toHaveLength(2);
    expect(notSpreadable[0]).toBe(1);
    // Element-wise: the array carries its own isConcatSpreadable symbol
    // property, which toEqual compares and a plain literal does not have.
    expect([...notSpreadable[1]]).toEqual(["a", "b"]);
  });
});

import { describe, expect, it } from "vitest";
import {
  indexedPairs,
  indexKeysOfArray,
  keysViaForIn,
  valuesViaForOf,
} from "@ex/01-beginner/ex031_for_of_vs_for_in/index.js";

describe("ex031 valuesViaForOf", () => {
  it("walks any iterable", () => {
    expect(valuesViaForOf([1, 2])).toEqual([1, 2]);
    expect(valuesViaForOf("ab")).toEqual(["a", "b"]);
    expect(valuesViaForOf(new Set([1, 1, 2]))).toEqual([1, 2]);
    expect(valuesViaForOf(new Map([["a", 1]]))).toEqual([["a", 1]]);
  });

  it("refuses a plain object, which is not iterable", () => {
    expect(() => valuesViaForOf({ a: 1 })).toThrow(TypeError);
  });
});

describe("ex031 keysViaForIn", () => {
  it("lists the own enumerable keys in order", () => {
    expect(keysViaForIn({ b: 1, a: 2 })).toEqual(["b", "a"]);
  });

  it("walks up the prototype chain", () => {
    // The reason for..in needs a hasOwn guard in real code.
    const parent = { inherited: 1 };
    const child = Object.create(parent);
    child.own = 2;
    expect(keysViaForIn(child)).toEqual(["own", "inherited"]);
  });

  it("skips non-enumerable keys, which is why class methods never appear", () => {
    class Thing {
      constructor() {
        this.data = 1;
      }

      method() {}
    }
    expect(keysViaForIn(new Thing())).toEqual(["data"]);

    const hidden = {};
    Object.defineProperty(hidden, "secret", { value: 1, enumerable: false });
    expect(keysViaForIn(hidden)).toEqual([]);
  });
});

describe("ex031 indexKeysOfArray", () => {
  it("yields string indices, not numbers", () => {
    expect(indexKeysOfArray(["a", "b"])).toEqual(["0", "1"]);
  });

  it("also yields a property somebody hung on the array", () => {
    const list = ["a"];
    list.label = "extra";
    expect(indexKeysOfArray(list)).toEqual(["0", "label"]);
  });

  it("skips holes, which for..of would report as undefined", () => {
    const sparse = [1, , 3];
    expect(indexKeysOfArray(sparse)).toEqual(["0", "2"]);
    expect(valuesViaForOf(sparse)).toEqual([1, undefined, 3]);
  });
});

describe("ex031 indexedPairs", () => {
  it("pairs index with value", () => {
    expect(indexedPairs(["a", "b"])).toEqual([
      [0, "a"],
      [1, "b"],
    ]);
  });

  it("gives numbers for the indices", () => {
    expect(typeof indexedPairs(["a"])[0][0]).toBe("number");
  });

  it("is empty for an empty list", () => {
    expect(indexedPairs([])).toEqual([]);
  });
});

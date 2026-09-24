import { describe, expect, it } from "vitest";
import {
  appendCopy,
  drainInto,
  insertAt,
  removeAt,
  takeFirst,
} from "@ex/01-beginner/ex007_array_mutation/index.js";

describe("ex007 appendCopy", () => {
  it("appends", () => {
    expect(appendCopy([1, 2], 3)).toEqual([1, 2, 3]);
    expect(appendCopy([], "a")).toEqual(["a"]);
  });

  it("leaves the input alone and returns a different array", () => {
    const input = [1, 2];
    const result = appendCopy(input, 3);
    expect(input).toEqual([1, 2]);
    expect(result).not.toBe(input);
  });
});

describe("ex007 removeAt", () => {
  it("removes the element at the index", () => {
    expect(removeAt(["a", "b", "c"], 1)).toEqual(["a", "c"]);
    expect(removeAt(["a"], 0)).toEqual([]);
  });

  it("removes nothing for an out-of-range index", () => {
    expect(removeAt(["a", "b"], 5)).toEqual(["a", "b"]);
    expect(removeAt(["a", "b"], -1)).toEqual(["a", "b"]);
  });

  it("copies rather than splicing in place", () => {
    const input = ["a", "b", "c"];
    const result = removeAt(input, 0);
    expect(input).toEqual(["a", "b", "c"]);
    expect(result).not.toBe(input);
  });
});

describe("ex007 insertAt", () => {
  it("inserts before the index", () => {
    expect(insertAt([1, 3], 1, 2)).toEqual([1, 2, 3]);
    expect(insertAt([1, 2], 0, 0)).toEqual([0, 1, 2]);
  });

  it("appends when the index is past the end", () => {
    expect(insertAt([1], 9, 2)).toEqual([1, 2]);
  });

  it("copies", () => {
    const input = [1, 3];
    expect(insertAt(input, 1, 2)).not.toBe(input);
    expect(input).toEqual([1, 3]);
  });
});

describe("ex007 drainInto", () => {
  it("returns the very array it was given, now longer", () => {
    const target = [1];
    const result = drainInto(target, [2, 3]);
    expect(result).toBe(target);
    expect(target).toEqual([1, 2, 3]);
  });

  it("leaves the source alone", () => {
    const source = [2, 3];
    drainInto([], source);
    expect(source).toEqual([2, 3]);
  });

  it("accepts any iterable source", () => {
    expect(drainInto([], new Set(["a", "b"]))).toEqual(["a", "b"]);
  });

  it("survives a source big enough to blow the argument limit", () => {
    // push(...source) with 200k arguments overflows the call stack. A loop
    // does not — which is the whole reason this assertion is here.
    const big = Array.from({ length: 200_000 }, (_, i) => i);
    expect(drainInto([], big)).toHaveLength(200_000);
  });
});

describe("ex007 takeFirst", () => {
  it("takes at most count", () => {
    expect(takeFirst([1, 2, 3], 2)).toEqual([1, 2]);
    expect(takeFirst([1, 2], 9)).toEqual([1, 2]);
    expect(takeFirst([1, 2], 0)).toEqual([]);
  });

  it("copies", () => {
    const input = [1, 2];
    expect(takeFirst(input, 2)).not.toBe(input);
  });
});

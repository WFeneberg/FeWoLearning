import { describe, expect, it } from "vitest";
import {
  arrayOf,
  codePoints,
  fromArrayLike,
  holesVsUndefined,
  sequence,
} from "@ex/02-intermediate/ex060_array_from/index.js";

describe("ex060 sequence", () => {
  it("counts from 0", () => {
    expect(sequence(3)).toEqual([0, 1, 2]);
    expect(sequence(1)).toEqual([0]);
    expect(sequence(0)).toEqual([]);
  });

  it("has no holes", () => {
    // A holey array reports its length but skips map/forEach.
    expect(sequence(3).map((n) => n + 1)).toEqual([1, 2, 3]);
    expect(Object.keys(sequence(3))).toEqual(["0", "1", "2"]);
  });
});

describe("ex060 fromArrayLike", () => {
  it("reads index properties up to length", () => {
    expect(fromArrayLike({ 0: "a", 1: "b", length: 2 })).toEqual(["a", "b"]);
  });

  it("fills missing indices with undefined", () => {
    expect(fromArrayLike({ 0: "a", length: 3 })).toEqual(["a", undefined, undefined]);
  });

  it("returns a real array", () => {
    expect(Array.isArray(fromArrayLike({ length: 0 }))).toBe(true);
  });

  it("prefers the iterator when there is one", () => {
    expect(fromArrayLike(new Set(["a", "b"]))).toEqual(["a", "b"]);
  });
});

describe("ex060 codePoints", () => {
  it("maps each character", () => {
    expect(codePoints("AB")).toEqual([65, 66]);
  });

  it("keeps an astral character whole", () => {
    // "😀" is two UTF-16 code units. Iterating by code point gives one
    // character; indexing by [] would give two halves.
    expect(codePoints("😀")).toEqual([128_512]);
    expect("😀".length).toBe(2);
  });

  it("is empty for an empty string", () => {
    expect(codePoints("")).toEqual([]);
  });
});

describe("ex060 arrayOf", () => {
  it("wraps its arguments", () => {
    expect(arrayOf(1, 2, 3)).toEqual([1, 2, 3]);
    expect(arrayOf()).toEqual([]);
  });

  it("wraps a single number as one element", () => {
    // Array(3) is [ , , ] with length 3; Array.of(3) is [3].
    expect(arrayOf(3)).toEqual([3]);
    expect(arrayOf(3)).toHaveLength(1);
    expect(new Array(3)).toHaveLength(3);
  });
});

describe("ex060 holesVsUndefined", () => {
  it("shows map skipping holes and visiting undefineds", () => {
    const { holes, filled } = holesVsUndefined();
    expect(holes).toHaveLength(3);
    expect(Object.keys(holes)).toEqual([]);
    expect(filled).toEqual([1, 1, 1]);
  });
});

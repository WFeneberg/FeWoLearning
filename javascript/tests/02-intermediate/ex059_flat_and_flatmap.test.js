import { describe, expect, it } from "vitest";
import {
  expand,
  filterMap,
  flattenDeep,
  flattenOnce,
} from "@ex/02-intermediate/ex059_flat_and_flatmap/index.js";

describe("ex059 flattenOnce", () => {
  it("flattens exactly one level", () => {
    expect(flattenOnce([[1], [2, [3]]])).toEqual([1, 2, [3]]);
  });

  it("leaves a flat array alone and copies it", () => {
    const input = [1, 2];
    const result = flattenOnce(input);
    expect(result).toEqual([1, 2]);
    expect(result).not.toBe(input);
  });

  it("drops holes", () => {
    expect(flattenOnce([1, , 2])).toEqual([1, 2]);
  });
});

describe("ex059 flattenDeep", () => {
  it("flattens to the bottom", () => {
    expect(flattenDeep([1, [2, [3, [4, [5]]]]])).toEqual([1, 2, 3, 4, 5]);
  });

  it("handles an unknown depth", () => {
    let nested = [42];
    for (let i = 0; i < 50; i++) nested = [nested];
    expect(flattenDeep(nested)).toEqual([42]);
  });

  it("returns an empty array for nested emptiness", () => {
    expect(flattenDeep([[], [[]], [[[]]]])).toEqual([]);
  });
});

describe("ex059 expand", () => {
  it("maps one item to many", () => {
    expect(expand([1, 2], (n) => [n, n * 10])).toEqual([1, 10, 2, 20]);
  });

  it("maps one item to none", () => {
    expect(expand([1, 2, 3], (n) => (n === 2 ? [] : [n]))).toEqual([1, 3]);
  });

  it("flattens only one level, unlike flattenDeep", () => {
    expect(expand([1], (n) => [[n]])).toEqual([[1]]);
  });

  it("passes the index to the callback", () => {
    expect(expand(["a", "b"], (value, index) => [`${index}:${value}`])).toEqual(["0:a", "1:b"]);
  });
});

describe("ex059 filterMap", () => {
  it("filters and maps in one pass", () => {
    expect(
      filterMap(
        [1, 2, 3, 4],
        (n) => n % 2 === 0,
        (n) => n * 100,
      ),
    ).toEqual([200, 400]);
  });

  it("can drop everything or keep everything", () => {
    expect(filterMap([1, 2], () => false, (n) => n)).toEqual([]);
    expect(filterMap([1, 2], () => true, (n) => n)).toEqual([1, 2]);
  });

  it("does not call the mapper for a rejected item", () => {
    const seen = [];
    filterMap(
      [1, 2, 3],
      (n) => n === 2,
      (n) => {
        seen.push(n);
        return n;
      },
    );
    expect(seen).toEqual([2]);
  });

  it("keeps a mapped result that is itself an array intact one level deep", () => {
    expect(filterMap([1], () => true, (n) => [n, n])).toEqual([[1, 1]]);
  });
});

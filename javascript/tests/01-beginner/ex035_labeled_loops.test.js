import { describe, expect, it } from "vitest";
import {
  findInGrid,
  firstCommonValue,
  sumCleanRows,
} from "@ex/01-beginner/ex035_labeled_loops/index.js";

/** A row that counts how many of its cells were read. */
function countingRow(values, onRead) {
  return new Proxy(values, {
    get(target, key, receiver) {
      if (typeof key === "string" && /^\d+$/.test(key)) onRead();
      return Reflect.get(target, key, receiver);
    },
  });
}

describe("ex035 findInGrid", () => {
  const grid = () => [
    [1, 2, 3],
    [4, 5, 6],
    [7, 8, 9],
  ];

  it("finds the coordinates", () => {
    expect(findInGrid(grid(), 1)).toEqual({ row: 0, col: 0 });
    expect(findInGrid(grid(), 5)).toEqual({ row: 1, col: 1 });
    expect(findInGrid(grid(), 9)).toEqual({ row: 2, col: 2 });
  });

  it("returns null when it is not there", () => {
    expect(findInGrid(grid(), 99)).toBeNull();
    expect(findInGrid([], 1)).toBeNull();
    expect(findInGrid([[], []], 1)).toBeNull();
  });

  it("stops the moment it finds it", () => {
    // Without the labeled break, the inner loop ends but the outer one
    // carries on — and a flag-checked version visits every remaining cell.
    let reads = 0;
    const counted = [
      countingRow([1, 2, 3], () => reads++),
      countingRow([4, 5, 6], () => reads++),
    ];
    expect(findInGrid(counted, 2)).toEqual({ row: 0, col: 1 });
    expect(reads).toBe(2);
  });

  it("finds the first occurrence when the value repeats", () => {
    expect(
      findInGrid(
        [
          [0, 7],
          [7, 0],
        ],
        7,
      ),
    ).toEqual({ row: 0, col: 1 });
  });
});

describe("ex035 sumCleanRows", () => {
  it("sums only the rows with no negative", () => {
    expect(
      sumCleanRows([
        [1, 2],
        [3, -1, 100],
        [4],
      ]),
    ).toBe(7);
  });

  it("drops the whole row, including the values before the negative", () => {
    expect(sumCleanRows([[100, 200, -1]])).toBe(0);
  });

  it("handles empty rows and an empty grid", () => {
    expect(sumCleanRows([[], [5]])).toBe(5);
    expect(sumCleanRows([])).toBe(0);
  });

  it("treats 0 as clean", () => {
    expect(sumCleanRows([[0, 0]])).toBe(0);
    expect(sumCleanRows([[0, 5]])).toBe(5);
  });
});

describe("ex035 firstCommonValue", () => {
  it("finds the first value present in every list", () => {
    expect(firstCommonValue([[1, 2, 3], [3, 2], [2, 9]])).toBe(2);
  });

  it("scans the first list in order", () => {
    expect(firstCommonValue([[3, 2], [2, 3], [3, 2]])).toBe(3);
  });

  it("returns null when there is no overlap", () => {
    expect(firstCommonValue([[1], [2]])).toBeNull();
    expect(firstCommonValue([[1], []])).toBeNull();
  });

  it("returns the first value when there is only one list", () => {
    expect(firstCommonValue([[7, 8]])).toBe(7);
  });

  it("returns null for no lists at all", () => {
    expect(firstCommonValue([])).toBeNull();
  });
});

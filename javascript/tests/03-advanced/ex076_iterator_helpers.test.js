import { describe, expect, it } from "vitest";
import {
  firstMatching,
  mapIterable,
  page,
  sumOfSquares,
} from "@ex/03-advanced/ex076_iterator_helpers/index.js";

/** An endless counter that records how many values were pulled out of it. */
function counted(state) {
  return (function* source() {
    let n = 0;
    while (true) {
      state.pulled += 1;
      yield n++;
    }
  })();
}

describe("ex076 firstMatching", () => {
  it("filters and limits", () => {
    expect(firstMatching([1, 2, 3, 4, 5, 6].values(), (n) => n % 2 === 0, 2)).toEqual([2, 4]);
  });

  it("stops early when the source runs out", () => {
    expect(firstMatching([1, 2].values(), () => true, 10)).toEqual([1, 2]);
  });

  it("works on an endless source", () => {
    const state = { pulled: 0 };
    expect(firstMatching(counted(state), (n) => n % 10 === 0, 3)).toEqual([0, 10, 20]);
  });

  it("pulls no further than it must", () => {
    // An eager implementation would never return above, and here it would
    // over-pull. 0..4 is five pulls for the first three even numbers.
    const state = { pulled: 0 };
    firstMatching(counted(state), (n) => n % 2 === 0, 3);
    expect(state.pulled).toBe(5);
  });

  it("takes nothing for a count of 0", () => {
    const state = { pulled: 0 };
    expect(firstMatching(counted(state), () => true, 0)).toEqual([]);
    expect(state.pulled).toBe(0);
  });
});

describe("ex076 sumOfSquares", () => {
  it("sums", () => {
    expect(sumOfSquares([1, 2, 3].values())).toBe(14);
  });

  it("is 0 for an empty source", () => {
    expect(sumOfSquares([].values())).toBe(0);
  });

  it("consumes the iterator exactly once", () => {
    const iterator = [1, 2].values();
    expect(sumOfSquares(iterator)).toBe(5);
    expect(iterator.next().done).toBe(true);
  });
});

describe("ex076 page", () => {
  it("windows the sequence", () => {
    expect(page([1, 2, 3, 4, 5].values(), 1, 2)).toEqual([2, 3]);
    expect(page([1, 2, 3].values(), 0, 2)).toEqual([1, 2]);
  });

  it("copes with an offset past the end", () => {
    expect(page([1, 2].values(), 5, 2)).toEqual([]);
  });

  it("pages an endless source", () => {
    const state = { pulled: 0 };
    expect(page(counted(state), 100, 3)).toEqual([100, 101, 102]);
  });
});

describe("ex076 mapIterable", () => {
  it("adapts an array, a Set and a string", () => {
    expect(mapIterable([1, 2], (n) => n * 2)).toEqual([2, 4]);
    expect(mapIterable(new Set(["a", "b"]), (s) => s.toUpperCase())).toEqual(["A", "B"]);
    expect(mapIterable("ab", (c) => c + c)).toEqual(["aa", "bb"]);
  });

  it("passes the index as the second argument, like Array#map", () => {
    expect(mapIterable(["a", "b"], (value, index) => `${index}${value}`)).toEqual(["0a", "1b"]);
  });

  it("handles an empty iterable", () => {
    expect(mapIterable([], (n) => n)).toEqual([]);
  });
});

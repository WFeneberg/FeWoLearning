import { describe, expect, it } from "vitest";
import { Range, sumOf, take } from "@ex/01-beginner/ex034_iterables/index";

describe("ex034 Range", () => {
  it("spreads into its half-open interval", () => {
    expect([...new Range(1, 4)]).toEqual([1, 2, 3]);
  });

  it("is empty when from equals to", () => {
    expect([...new Range(3, 3)]).toEqual([]);
  });

  it("works with for…of", () => {
    const seen: number[] = [];
    for (const value of new Range(0, 3)) {
      seen.push(value);
    }
    expect(seen).toEqual([0, 1, 2]);
  });

  it("can be iterated twice, each from the start", () => {
    const range = new Range(0, 3);
    expect([...range]).toEqual([0, 1, 2]);
    expect([...range]).toEqual([0, 1, 2]);
  });
});

describe("ex034 sumOf", () => {
  it("adds up an array", () => {
    expect(sumOf([1, 2, 3])).toBe(6);
  });

  it("adds up a Set, which is iterable but not an array", () => {
    expect(sumOf(new Set([1, 2, 3, 3]))).toBe(6);
  });

  it("adds up a Range", () => {
    expect(sumOf(new Range(1, 5))).toBe(10);
  });

  it("is 0 for nothing", () => {
    expect(sumOf([])).toBe(0);
  });
});

describe("ex034 take", () => {
  it("takes a prefix", () => {
    expect(take(["a", "b", "c"], 2)).toEqual(["a", "b"]);
  });

  it("takes everything when asked for more than there is", () => {
    expect(take([1, 2], 5)).toEqual([1, 2]);
  });

  it("takes nothing for a count of zero", () => {
    expect(take([1, 2], 0)).toEqual([]);
  });

  // Stopping early is the point: this generator never ends.
  it("stops early rather than draining an endless iterable", () => {
    function* naturals(): Generator<number> {
      let n = 0;
      while (true) {
        yield n;
        n += 1;
      }
    }
    expect(take(naturals(), 4)).toEqual([0, 1, 2, 3]);
  });
});

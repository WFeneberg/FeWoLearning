import { describe, expect, it } from "vitest";
import {
  isIterable,
  makeRange,
  pullValues,
} from "@ex/01-beginner/ex032_iterable_protocol/index.js";

describe("ex032 makeRange", () => {
  it("works with for..of", () => {
    const seen = [];
    for (const value of makeRange(0, 3)) seen.push(value);
    expect(seen).toEqual([0, 1, 2]);
  });

  it("works with everything else that consumes an iterable", () => {
    expect([...makeRange(1, 4)]).toEqual([1, 2, 3]);
    expect(Array.from(makeRange(0, 2))).toEqual([0, 1]);
    const [first, second] = makeRange(5, 99);
    expect([first, second]).toEqual([5, 6]);
    expect(new Set(makeRange(0, 3)).size).toBe(3);
  });

  it("honours the step", () => {
    expect([...makeRange(0, 10, 3)]).toEqual([0, 3, 6, 9]);
  });

  it("is empty when end is not above start", () => {
    expect([...makeRange(5, 5)]).toEqual([]);
    expect([...makeRange(5, 1)]).toEqual([]);
  });

  it("can be iterated twice — the cursor belongs to the iterator", () => {
    // A range that keeps its counter on the object itself passes every
    // assertion above and fails this one.
    const range = makeRange(0, 3);
    expect([...range]).toEqual([0, 1, 2]);
    expect([...range]).toEqual([0, 1, 2]);
  });

  it("hands out a new iterator per call", () => {
    const range = makeRange(0, 3);
    expect(range[Symbol.iterator]()).not.toBe(range[Symbol.iterator]());
  });
});

describe("ex032 isIterable", () => {
  it("accepts the built-in iterables", () => {
    expect(isIterable([])).toBe(true);
    expect(isIterable("")).toBe(true);
    expect(isIterable(new Map())).toBe(true);
    expect(isIterable(new Set())).toBe(true);
    expect(isIterable(makeRange(0, 1))).toBe(true);
  });

  it("rejects the things that are not", () => {
    expect(isIterable({})).toBe(false);
    expect(isIterable(42)).toBe(false);
    expect(isIterable(null)).toBe(false);
    expect(isIterable(undefined)).toBe(false);
    expect(isIterable({ [Symbol.iterator]: "not callable" })).toBe(false);
  });
});

describe("ex032 pullValues", () => {
  it("pulls exactly as many as asked", () => {
    expect(pullValues([1, 2, 3, 4], 2)).toEqual([1, 2]);
    expect(pullValues("abc", 0)).toEqual([]);
  });

  it("stops when the iterator is exhausted", () => {
    expect(pullValues([1, 2], 5)).toEqual([1, 2]);
  });

  it("does not exhaust an endless source", () => {
    // Bounded by the consumer, never by the producer: a spread here would
    // never return.
    const endless = {
      [Symbol.iterator]() {
        let n = 0;
        return { next: () => ({ value: n++, done: false }) };
      },
    };
    expect(pullValues(endless, 4)).toEqual([0, 1, 2, 3]);
  });
});

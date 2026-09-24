import { describe, expect, it, vi } from "vitest";
import {
  countedRange,
  naturals,
  range,
  take,
} from "@ex/01-beginner/ex033_generators/index.js";

describe("ex033 range", () => {
  it("yields the values", () => {
    expect([...range(0, 3)]).toEqual([0, 1, 2]);
    expect([...range(2, 9, 3)]).toEqual([2, 5, 8]);
  });

  it("is empty when there is nothing to yield", () => {
    expect([...range(3, 3)]).toEqual([]);
  });

  it("returns a generator, which is an iterator AND iterable", () => {
    const generated = range(0, 2);
    // Anchored: a generator function is one from the moment it is written.
    expect(generated.next().value).toBe(0);
    expect(typeof generated.next).toBe("function");
    expect(generated[Symbol.iterator]()).toBe(generated);
  });

  it("is single-use, unlike ex032's range object", () => {
    const generated = range(0, 3);
    expect([...generated]).toEqual([0, 1, 2]);
    expect([...generated]).toEqual([]);
  });
});

describe("ex033 naturals", () => {
  it("starts at 0 and keeps going", () => {
    const values = naturals();
    expect(values.next().value).toBe(0);
    expect(values.next().value).toBe(1);
    expect(values.next().value).toBe(2);
  });

  it("never reports done", () => {
    expect(naturals().next().done).toBe(false);
  });
});

describe("ex033 take", () => {
  it("takes from a finite source", () => {
    expect([...take([1, 2, 3], 2)]).toEqual([1, 2]);
    expect([...take([1, 2], 5)]).toEqual([1, 2]);
    expect([...take([1, 2], 0)]).toEqual([]);
  });

  it("takes from an endless one without hanging", () => {
    expect([...take(naturals(), 4)]).toEqual([0, 1, 2, 3]);
  });

  it("pulls no more than it needs", () => {
    // The laziness check: a `[...iterable].slice(0, count)` implementation
    // passes every assertion above and fails this one — and would never
    // return at all for the endless case.
    let pulled = 0;
    const counted = (function* () {
      while (true) {
        pulled++;
        yield pulled;
      }
    })();
    expect([...take(counted, 3)]).toEqual([1, 2, 3]);
    expect(pulled).toBe(3);
  });
});

describe("ex033 countedRange", () => {
  it("runs nothing until the first pull", () => {
    const onEnter = vi.fn();
    const generated = countedRange(onEnter);
    expect(onEnter).not.toHaveBeenCalled();
    expect(generated.next().value).toBe(1);
    expect(onEnter).toHaveBeenCalledTimes(1);
  });

  it("does not re-enter the body on later pulls", () => {
    const onEnter = vi.fn();
    const generated = countedRange(onEnter);
    expect([...generated]).toEqual([1, 2]);
    expect(onEnter).toHaveBeenCalledTimes(1);
  });
});

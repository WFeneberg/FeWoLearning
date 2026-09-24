import { describe, expect, it, vi } from "vitest";
import {
  compose,
  filtering,
  into,
  isReduced,
  mapping,
  reduced,
  taking,
  transduce,
} from "@ex/04-expert/ex097_transducers/index.js";

describe("ex097 reduced", () => {
  it("wraps and recognises", () => {
    expect(isReduced(reduced(1))).toBe(true);
    expect(isReduced(1)).toBe(false);
    expect(isReduced(null)).toBe(false);
    expect(isReduced({ value: 1 })).toBe(false);
  });
});

describe("ex097 single transducers", () => {
  it("maps", () => {
    expect(into(mapping((n) => n * 2), [1, 2, 3])).toEqual([2, 4, 6]);
  });

  it("filters", () => {
    expect(into(filtering((n) => n % 2 === 0), [1, 2, 3, 4])).toEqual([2, 4]);
  });

  it("takes", () => {
    expect(into(taking(2), [1, 2, 3, 4])).toEqual([1, 2]);
    expect(into(taking(0), [1, 2])).toEqual([]);
    expect(into(taking(9), [1])).toEqual([1]);
  });
});

describe("ex097 compose", () => {
  it("applies to the data left to right", () => {
    const xform = compose(
      mapping((n) => n * 10),
      filtering((n) => n > 15),
    );
    expect(into(xform, [1, 2, 3])).toEqual([20, 30]);
  });

  it("is order-sensitive, as a pipeline should be", () => {
    const mapThenFilter = compose(
      mapping((n) => n + 1),
      filtering((n) => n % 2 === 0),
    );
    const filterThenMap = compose(
      filtering((n) => n % 2 === 0),
      mapping((n) => n + 1),
    );
    expect(into(mapThenFilter, [1, 2, 3, 4])).toEqual([2, 4]);
    expect(into(filterThenMap, [1, 2, 3, 4])).toEqual([3, 5]);
  });

  it("composes three, including take", () => {
    const xform = compose(
      filtering((n) => n % 2 === 0),
      mapping((n) => n * 100),
      taking(2),
    );
    expect(into(xform, [1, 2, 3, 4, 5, 6, 7, 8])).toEqual([200, 400]);
  });
});

describe("ex097 one pass, early exit", () => {
  it("touches each element once", () => {
    // A chain of array methods walks the list three times and allocates
    // two intermediate arrays; a transducer does neither.
    const map = vi.fn((n) => n * 2);
    const keep = vi.fn((n) => n > 2);
    into(compose(mapping(map), filtering(keep)), [1, 2, 3]);
    expect(map).toHaveBeenCalledTimes(3);
    expect(keep).toHaveBeenCalledTimes(3);
  });

  it("stops pulling once take is satisfied", () => {
    const map = vi.fn((n) => n);
    into(compose(mapping(map), taking(2)), [1, 2, 3, 4, 5]);
    expect(map).toHaveBeenCalledTimes(2);
  });

  it("does not evaluate a filter after take has finished", () => {
    const keep = vi.fn(() => true);
    into(compose(taking(1), filtering(keep)), [1, 2, 3]);
    expect(keep).toHaveBeenCalledTimes(1);
  });

  it("terminates on an endless source", () => {
    const naturals = (function* endless() {
      let n = 0;
      while (true) yield n++;
    })();
    expect(into(taking(3), naturals)).toEqual([0, 1, 2]);
  });

  it("gives each run its own take counter", () => {
    const xform = taking(2);
    expect(into(xform, [1, 2, 3])).toEqual([1, 2]);
    expect(into(xform, [4, 5, 6])).toEqual([4, 5]);
  });
});

describe("ex097 transduce", () => {
  it("works with any reducer", () => {
    const sum = transduce(mapping((n) => n * 2), (total, n) => total + n, 0, [1, 2, 3]);
    expect(sum).toBe(12);
  });

  it("returns the initial value for an empty input", () => {
    expect(transduce(mapping((n) => n), (total, n) => total + n, 100, [])).toBe(100);
  });

  it("unwraps a reduced result", () => {
    const sum = transduce(taking(2), (total, n) => total + n, 0, [1, 2, 3, 4]);
    expect(sum).toBe(3);
  });

  it("accepts any iterable", () => {
    expect(into(mapping((c) => c.toUpperCase()), "ab")).toEqual(["A", "B"]);
    expect(into(mapping((n) => n), new Set([1, 2]))).toEqual([1, 2]);
  });
});

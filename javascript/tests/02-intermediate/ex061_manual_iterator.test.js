import { describe, expect, it, vi } from "vitest";
import { chain, closeable, countdown } from "@ex/02-intermediate/ex061_manual_iterator/index.js";

describe("ex061 countdown", () => {
  it("counts down", () => {
    expect([...countdown(3)]).toEqual([3, 2, 1]);
    expect([...countdown(1)]).toEqual([1]);
    expect([...countdown(0)]).toEqual([]);
  });

  it("is re-iterable", () => {
    const counter = countdown(2);
    expect([...counter]).toEqual([2, 1]);
    expect([...counter]).toEqual([2, 1]);
  });

  it("is not a generator — its iterator has no throw()", () => {
    const iterator = countdown(1)[Symbol.iterator]();
    expect(iterator.next()).toEqual({ value: 1, done: false });
    expect(iterator.throw).toBeUndefined();
  });
});

describe("ex061 closeable", () => {
  it("iterates the values", () => {
    expect([...closeable([1, 2], () => {})]).toEqual([1, 2]);
  });

  it("closes on break", () => {
    const onClose = vi.fn();
    for (const value of closeable([1, 2, 3], onClose)) {
      if (value === 2) break;
    }
    expect(onClose).toHaveBeenCalledTimes(1);
  });

  it("closes on a throw out of the loop body", () => {
    const onClose = vi.fn();
    expect(() => {
      for (const value of closeable([1, 2], onClose)) {
        throw new Error(`stopped at ${value}`);
      }
    }).toThrow("stopped at 1");
    expect(onClose).toHaveBeenCalledTimes(1);
  });

  it("closes on a partial destructuring", () => {
    const onClose = vi.fn();
    const [first] = closeable([1, 2, 3], onClose);
    expect(first).toBe(1);
    expect(onClose).toHaveBeenCalledTimes(1);
  });

  it("does NOT close when the loop ran to the end", () => {
    // return() is the early-exit hook; a completed iterator was never asked
    // to stop.
    const onClose = vi.fn();
    expect([...closeable([1, 2], onClose)]).toEqual([1, 2]);
    expect(onClose).not.toHaveBeenCalled();
  });

  it("closes only once even if return() is called again", () => {
    const onClose = vi.fn();
    const iterator = closeable([1, 2], onClose)[Symbol.iterator]();
    iterator.next();
    iterator.return();
    iterator.return();
    expect(onClose).toHaveBeenCalledTimes(1);
  });
});

describe("ex061 chain", () => {
  it("runs through every source in order", () => {
    expect([...chain([1, 2], "ab", new Set([9]))]).toEqual([1, 2, "a", "b", 9]);
  });

  it("skips empty sources", () => {
    expect([...chain([], [1], [])]).toEqual([1]);
    expect([...chain()]).toEqual([]);
  });

  it("stays lazy — it does not drain a source it has not reached", () => {
    let pulled = 0;
    const counting = {
      [Symbol.iterator]() {
        let n = 0;
        return {
          next() {
            pulled += 1;
            return n < 3 ? { value: n++, done: false } : { value: undefined, done: true };
          },
        };
      },
    };
    const iterator = chain([1], counting)[Symbol.iterator]();
    expect(iterator.next().value).toBe(1);
    expect(pulled).toBe(0);
  });

  it("is re-iterable", () => {
    const chained = chain([1], [2]);
    expect([...chained]).toEqual([1, 2]);
    expect([...chained]).toEqual([1, 2]);
  });
});

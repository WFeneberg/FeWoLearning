import { describe, expect, it, vi } from "vitest";
import { indexReaders, makeCounter, once } from "@ex/01-beginner/ex016_closures/index.js";

describe("ex016 makeCounter", () => {
  it("counts from the start value", () => {
    const counter = makeCounter(10);
    expect(counter.next()).toBe(10);
    expect(counter.next()).toBe(11);
  });

  it("defaults to 0 and resets to the start", () => {
    const counter = makeCounter();
    counter.next();
    counter.next();
    counter.reset();
    expect(counter.next()).toBe(0);
  });

  it("gives each counter its own state", () => {
    const a = makeCounter();
    const b = makeCounter();
    a.next();
    a.next();
    expect(b.next()).toBe(0);
  });

  it("keeps the count out of the object", () => {
    // If the count were a property, this would find it — and anyone could
    // set it. Closed-over state is the only privacy that predates #fields.
    const counter = makeCounter(5);
    counter.next();
    expect(Object.values(counter).every((value) => typeof value === "function")).toBe(true);
    expect(JSON.stringify(counter)).toBe("{}");
  });
});

describe("ex016 once", () => {
  it("calls the function exactly once", () => {
    const fn = vi.fn(() => "result");
    const wrapped = once(fn);
    expect(wrapped()).toBe("result");
    expect(wrapped()).toBe("result");
    expect(wrapped()).toBe("result");
    expect(fn).toHaveBeenCalledTimes(1);
  });

  it("passes the first call's arguments through", () => {
    const fn = vi.fn((a, b) => a + b);
    expect(once(fn)(2, 3)).toBe(5);
    expect(fn).toHaveBeenCalledWith(2, 3);
  });

  it("remembers a first result of undefined instead of retrying", () => {
    // The trap: caching with `result ??= fn()` re-runs forever here.
    const fn = vi.fn(() => undefined);
    const wrapped = once(fn);
    expect(wrapped()).toBeUndefined();
    expect(wrapped()).toBeUndefined();
    expect(fn).toHaveBeenCalledTimes(1);
  });

  it("wraps each function separately", () => {
    const first = vi.fn();
    const second = vi.fn();
    once(first)();
    once(second)();
    expect(first).toHaveBeenCalledTimes(1);
    expect(second).toHaveBeenCalledTimes(1);
  });
});

describe("ex016 indexReaders", () => {
  it("gives each function its own index", () => {
    expect(indexReaders(3).map((read) => read())).toEqual([0, 1, 2]);
  });

  it("does not hand every function the final value", () => {
    // [3, 3, 3] is what a single `var i` binding produces.
    expect(indexReaders(3).map((read) => read())).not.toEqual([3, 3, 3]);
  });

  it("returns an empty array for 0", () => {
    expect(indexReaders(0)).toEqual([]);
  });
});

import { describe, expect, it } from "vitest";
import { increment, ticker } from "@ex/02-intermediate/ex062_this_typing/index";
import type { Counter } from "@ex/02-intermediate/ex062_this_typing/index";

describe("ex062 increment", () => {
  it("adds to the receiver and returns the new count", () => {
    const counter: Counter = { count: 5 };
    expect(increment.call(counter, 3)).toBe(8);
    expect(counter.count).toBe(8);
  });

  it("accumulates across calls", () => {
    const counter: Counter = { count: 0 };
    increment.call(counter, 1);
    increment.call(counter, 2);
    expect(counter.count).toBe(3);
  });

  // A fact asserting `increment.length === 1` was written and dropped: the
  // `this` parameter is erased, so the count is 1 with or without it and
  // the fact is green on the untouched stub. The erasure is real; it is
  // simply not observable from here.
});

describe("ex062 ticker", () => {
  it("increments each time it is called", () => {
    const counter: Counter = { count: 0 };
    const tick = ticker(counter);
    expect([tick(), tick(), tick()]).toEqual([1, 2, 3]);
    expect(counter.count).toBe(3);
  });

  // The oldest trap in JavaScript, from the other side: this one survives
  // being passed around, because it closed over the counter rather than
  // relying on a receiver.
  it("keeps working after being detached and passed on", () => {
    const counter: Counter = { count: 10 };
    const tick = ticker(counter);
    const run = (fn: () => number): number => fn();
    expect(run(tick)).toBe(11);
  });

  it("gives separate tickers separate counters", () => {
    const a: Counter = { count: 0 };
    const b: Counter = { count: 100 };
    const tickA = ticker(a);
    const tickB = ticker(b);
    tickA();
    expect([a.count, b.count]).toEqual([1, 100]);
    tickB();
    expect([a.count, b.count]).toEqual([1, 101]);
  });
});

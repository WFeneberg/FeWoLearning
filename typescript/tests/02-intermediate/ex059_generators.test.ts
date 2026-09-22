import { describe, expect, it } from "vitest";
import { countTo, runningTotal } from "@ex/02-intermediate/ex059_generators/index";

describe("ex059 countTo", () => {
  it("yields every value up to n", () => {
    expect([...countTo(4)]).toEqual([1, 2, 3, 4]);
  });

  it("yields nothing for zero", () => {
    expect([...countTo(0)]).toEqual([]);
  });

  // The RETURN value lands on the step where done becomes true — and is
  // discarded by for…of and by spread, which is why it needs next().
  it("returns the total on the final step", () => {
    const gen = countTo(3);
    expect(gen.next()).toEqual({ value: 1, done: false });
    expect(gen.next()).toEqual({ value: 2, done: false });
    expect(gen.next()).toEqual({ value: 3, done: false });
    expect(gen.next()).toEqual({ value: 6, done: true });
  });
});

describe("ex059 runningTotal", () => {
  it("yields the starting total before anything is sent", () => {
    const gen = runningTotal(10);
    expect(gen.next().value).toBe(10);
  });

  // The first next() has no suspended yield to deliver to, so its
  // argument is discarded — the conversation starts with the second call.
  it("adds each value the consumer sends", () => {
    const gen = runningTotal(0);
    expect(gen.next().value).toBe(0);
    expect(gen.next(5).value).toBe(5);
    expect(gen.next(3).value).toBe(8);
    expect(gen.next(-8).value).toBe(0);
  });

  it("discards the argument of the very first next", () => {
    const gen = runningTotal(7);
    expect(gen.next(100).value).toBe(7);
  });

  it("never finishes on its own", () => {
    const gen = runningTotal(0);
    gen.next();
    expect(gen.next(1).done).toBe(false);
  });
});

import { describe, expect, it } from "vitest";
import { ManualRange, closable } from "@ex/02-intermediate/ex061_iterator_protocol/index";

describe("ex061 ManualRange", () => {
  it("iterates the half-open interval", () => {
    expect([...new ManualRange(1, 4)]).toEqual([1, 2, 3]);
  });

  it("is empty when from equals to", () => {
    expect([...new ManualRange(3, 3)]).toEqual([]);
  });

  // A fresh iterator per call is what makes this possible; an iterable
  // that returns itself gives nothing the second time.
  it("can be walked twice, each time from the start", () => {
    const range = new ManualRange(0, 3);
    expect([...range]).toEqual([0, 1, 2]);
    expect([...range]).toEqual([0, 1, 2]);
  });

  it("works with for…of", () => {
    const seen: number[] = [];
    for (const value of new ManualRange(5, 8)) {
      seen.push(value);
    }
    expect(seen).toEqual([5, 6, 7]);
  });
});

describe("ex061 closable", () => {
  it("yields its values", () => {
    expect([...closable([1, 2, 3], () => undefined)]).toEqual([1, 2, 3]);
  });

  // A full drain does NOT call return(): the iterator already said done.
  it("does not close on a full drain", () => {
    let closed = 0;
    for (const _ of closable([1, 2], () => (closed += 1))) {
      void _;
    }
    expect(closed).toBe(0);
  });

  // Leaving early does. This is the protocol's cleanup hook.
  it("closes when the consumer breaks out", () => {
    let closed = 0;
    for (const value of closable([1, 2, 3], () => (closed += 1))) {
      if (value === 2) {
        break;
      }
    }
    expect(closed).toBe(1);
  });

  it("closes only once", () => {
    let closed = 0;
    const iterator = closable([1, 2, 3], () => (closed += 1));
    iterator.return?.();
    iterator.return?.();
    expect(closed).toBe(1);
  });

  // It returns itself, so it is single-use.
  it("is exhausted after one walk", () => {
    const iterator = closable([1, 2], () => undefined);
    expect([...iterator]).toEqual([1, 2]);
    expect([...iterator]).toEqual([]);
  });
});

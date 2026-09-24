import { describe, expect, it } from "vitest";
import {
  countWithAtomics,
  createRing,
  tryCompareExchange,
} from "@ex/04-expert/ex099_atomics_ring_buffer/index.js";

describe("ex099 createRing", () => {
  it("pushes and shifts in FIFO order", () => {
    const ring = createRing(4);
    expect(ring.push(1)).toBe(true);
    expect(ring.push(2)).toBe(true);
    expect(ring.shift()).toBe(1);
    expect(ring.shift()).toBe(2);
  });

  it("is empty at the start and after draining", () => {
    const ring = createRing(2);
    expect(ring.size).toBe(0);
    expect(ring.shift()).toBeUndefined();
    ring.push(1);
    ring.shift();
    expect(ring.size).toBe(0);
    expect(ring.shift()).toBeUndefined();
  });

  it("reports its size", () => {
    const ring = createRing(3);
    ring.push(1);
    ring.push(2);
    expect(ring.size).toBe(2);
    ring.shift();
    expect(ring.size).toBe(1);
  });

  it("refuses to overfill", () => {
    const ring = createRing(2);
    expect(ring.push(1)).toBe(true);
    expect(ring.push(2)).toBe(true);
    expect(ring.push(3)).toBe(false);
    expect(ring.size).toBe(2);
  });

  it("keeps the earlier values when a push is refused", () => {
    const ring = createRing(2);
    ring.push(1);
    ring.push(2);
    ring.push(3);
    expect([ring.shift(), ring.shift()]).toEqual([1, 2]);
  });

  it("wraps around correctly", () => {
    // The place a hand-written ring is wrong: full and empty both have
    // read and write meeting, unless one slot is left unused.
    const ring = createRing(3);
    for (let round = 0; round < 10; round++) {
      expect(ring.push(round)).toBe(true);
      expect(ring.shift()).toBe(round);
      expect(ring.size).toBe(0);
    }
  });

  it("survives filling, draining and filling again", () => {
    const ring = createRing(3);
    for (const value of [1, 2, 3]) ring.push(value);
    expect(ring.push(4)).toBe(false);
    expect([ring.shift(), ring.shift(), ring.shift()]).toEqual([1, 2, 3]);
    for (const value of [4, 5, 6]) expect(ring.push(value)).toBe(true);
    expect([ring.shift(), ring.shift(), ring.shift()]).toEqual([4, 5, 6]);
  });

  it("reports its capacity", () => {
    expect(createRing(5).capacity).toBe(5);
  });

  it("holds int32 values", () => {
    const ring = createRing(2);
    ring.push(-2_147_483_648);
    ring.push(2_147_483_647);
    expect([ring.shift(), ring.shift()]).toEqual([-2_147_483_648, 2_147_483_647]);
  });
});

describe("ex099 countWithAtomics", () => {
  it("counts", () => {
    expect(countWithAtomics(0)).toBe(0);
    expect(countWithAtomics(1000)).toBe(1000);
  });
});

describe("ex099 tryCompareExchange", () => {
  it("writes when the expected value is still there", () => {
    const view = new Int32Array(new SharedArrayBuffer(4));
    Atomics.store(view, 0, 5);
    expect(tryCompareExchange(view, 5, 9)).toEqual({ previous: 5, written: true });
    expect(Atomics.load(view, 0)).toBe(9);
  });

  it("leaves the slot alone when it is not", () => {
    const view = new Int32Array(new SharedArrayBuffer(4));
    Atomics.store(view, 0, 5);
    expect(tryCompareExchange(view, 1, 9)).toEqual({ previous: 5, written: false });
    expect(Atomics.load(view, 0)).toBe(5);
  });

  it("works on a plain Int32Array too", () => {
    const view = new Int32Array(1);
    expect(tryCompareExchange(view, 0, 3).written).toBe(true);
    expect(view[0]).toBe(3);
  });
});

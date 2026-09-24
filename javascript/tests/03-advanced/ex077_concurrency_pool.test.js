import { describe, expect, it, vi } from "vitest";
import {
  mapWithConcurrency,
  settleWithConcurrency,
} from "@ex/03-advanced/ex077_concurrency_pool/index.js";

/** A worker whose calls only finish when the test releases them. */
function gated() {
  const waiting = [];
  const worker = (item) =>
    new Promise((resolve) => {
      waiting.push(() => resolve(item));
    });
  return { worker, waiting };
}

/** A worker that records the highest number of simultaneous calls. */
function tracking() {
  const state = { inFlight: 0, peak: 0 };
  const worker = async (item) => {
    state.peak = Math.max(state.peak, ++state.inFlight);
    await Promise.resolve();
    await Promise.resolve();
    state.inFlight -= 1;
    return item * 2;
  };
  return { worker, state };
}

describe("ex077 mapWithConcurrency", () => {
  it("returns results in input order", async () => {
    const { worker } = tracking();
    await expect(mapWithConcurrency([1, 2, 3, 4, 5], 2, worker)).resolves.toEqual([
      2, 4, 6, 8, 10,
    ]);
  });

  it("never exceeds the limit", async () => {
    const { worker, state } = tracking();
    await mapWithConcurrency([1, 2, 3, 4, 5, 6], 2, worker);
    expect(state.peak).toBe(2);
  });

  it("actually uses the limit rather than going one at a time", async () => {
    const { worker, state } = tracking();
    await mapWithConcurrency([1, 2, 3, 4, 5, 6], 3, worker);
    expect(state.peak).toBe(3);
  });

  it("starts exactly `limit` tasks up front and refills as they settle", async () => {
    const { worker, waiting } = gated();
    const all = mapWithConcurrency([1, 2, 3, 4], 2, worker);
    await Promise.resolve();
    expect(waiting).toHaveLength(2);

    waiting.shift()();
    await Promise.resolve();
    await Promise.resolve();
    expect(waiting).toHaveLength(2);

    // Bounded drain: releasing the current waiters only schedules the next
    // ones a microtask later, so a single sweep leaves the pool half-fed
    // and the test hanging rather than failing.
    let guard = 0;
    while (waiting.length > 0 && guard++ < 50) {
      waiting.shift()();
      await Promise.resolve();
      await Promise.resolve();
    }
    await expect(all).resolves.toEqual([1, 2, 3, 4]);
  });

  it("never starts more tasks than there are items", async () => {
    const worker = vi.fn(async (item) => item);
    await mapWithConcurrency([1], 10, worker);
    expect(worker).toHaveBeenCalledTimes(1);
  });

  it("passes the index to the worker", async () => {
    await expect(mapWithConcurrency(["a", "b"], 1, async (item, index) => `${index}${item}`))
      .resolves.toEqual(["0a", "1b"]);
  });

  it("rejects on a worker failure without hanging", async () => {
    await expect(
      mapWithConcurrency([1, 2, 3], 2, async (item) => {
        if (item === 2) throw new Error("worker failed");
        return item;
      }),
    ).rejects.toThrow("worker failed");
  });

  it("handles an empty list", async () => {
    await expect(mapWithConcurrency([], 3, async (n) => n)).resolves.toEqual([]);
  });
});

describe("ex077 settleWithConcurrency", () => {
  it("reports both outcomes in input order", async () => {
    const results = await settleWithConcurrency([1, 2, 3], 2, async (item) => {
      if (item === 2) throw new Error("no");
      return item;
    });
    expect(results.map((r) => r.status)).toEqual(["fulfilled", "rejected", "fulfilled"]);
    expect(results[0].value).toBe(1);
    expect(results[1].reason.message).toBe("no");
  });

  it("keeps running after a failure", async () => {
    const worker = vi.fn(async () => {
      throw new Error("always");
    });
    const results = await settleWithConcurrency([1, 2, 3], 2, worker);
    expect(results).toHaveLength(3);
    expect(worker).toHaveBeenCalledTimes(3);
  });

  it("respects the limit too", async () => {
    const { worker, state } = tracking();
    await settleWithConcurrency([1, 2, 3, 4, 5], 2, worker);
    expect(state.peak).toBe(2);
  });
});

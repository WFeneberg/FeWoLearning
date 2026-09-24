import { describe, expect, it } from "vitest";
import {
  afterTurns,
  microtaskStarvation,
  recordOrder,
} from "@ex/02-intermediate/ex039_job_queue_ordering/index.js";

describe("ex039 recordOrder", () => {
  it("runs sync first, then the microtasks in queue order, then the timer", async () => {
    // Source order was timeout, promise, microtask, sync — and none of that
    // matters except among the two microtasks.
    await expect(recordOrder()).resolves.toEqual(["sync", "promise", "microtask", "timeout"]);
  });
});

describe("ex039 microtaskStarvation", () => {
  it("drains a chain of microtasks before a timer that was already due", async () => {
    const log = await microtaskStarvation(50);
    expect(log).toHaveLength(51);
    expect(log.at(-1)).toBe("timer");
    expect(log.slice(0, 50).every((entry) => entry === "micro")).toBe(true);
  });

  it("still fires the timer when there are no microtasks at all", async () => {
    await expect(microtaskStarvation(0)).resolves.toEqual(["timer"]);
  });
});

describe("ex039 afterTurns", () => {
  it("resolves to the number of turns", async () => {
    await expect(afterTurns(3)).resolves.toBe(3);
    await expect(afterTurns(0)).resolves.toBe(0);
  });

  it("settles in turn order", async () => {
    const log = [];
    await Promise.all([
      afterTurns(3).then(() => log.push("three")),
      afterTurns(1).then(() => log.push("one")),
    ]);
    expect(log).toEqual(["one", "three"]);
  });

  it("uses microtasks, not timers — every turn still beats a due timer", async () => {
    const log = [];
    await new Promise((resolve) => {
      setTimeout(() => {
        log.push("timer");
        resolve();
      }, 0);
      afterTurns(5).then(() => log.push("turns"));
    });
    expect(log).toEqual(["turns", "timer"]);
  });
});

import { describe, expect, it } from "vitest";
import {
  awaitAlwaysYields,
  scheduleAll,
  timerRanDuringBusyWait,
} from "@ex/01-beginner/ex032_event_loop_ordering/index";

describe("ex032 scheduleAll", () => {
  it("runs synchronous code, then both microtasks, then the timer", async () => {
    await expect(scheduleAll()).resolves.toEqual(["sync", "micro", "promise", "macro"]);
  });
});

describe("ex032 awaitAlwaysYields", () => {
  // In C# an await on a completed Task continues synchronously and this
  // would be a, b, c. Here `await 1` suspends anyway.
  it("resumes after the caller, even awaiting a plain value", async () => {
    await expect(awaitAlwaysYields()).resolves.toEqual(["a", "c", "b"]);
  });
});

describe("ex032 timerRanDuringBusyWait", () => {
  it("never lets the timer run during the loop", () => {
    expect(timerRanDuringBusyWait(5)).toBe(false);
  });

  it("gives the same answer however long the loop spins", () => {
    expect(timerRanDuringBusyWait(25)).toBe(false);
  });

  it("really does wait, rather than returning early", () => {
    const before = Date.now();
    timerRanDuringBusyWait(20);
    expect(Date.now() - before).toBeGreaterThanOrEqual(19);
  });
});

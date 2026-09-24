import { describe, expect, it, vi } from "vitest";
import {
  runBoth,
  runParallel,
  runParallelIgnoringFailures,
  runSequential,
} from "@ex/02-intermediate/ex040_sequential_vs_parallel/index.js";

/**
 * Builds `count` tasks that resolve to their own index, and reports the
 * highest number that were ever in flight at the same moment.
 */
function tracked(count) {
  const state = { peak: 0, inFlight: 0, order: [] };
  const tasks = Array.from({ length: count }, (_, index) => async () => {
    state.inFlight += 1;
    state.peak = Math.max(state.peak, state.inFlight);
    state.order.push(index);
    await Promise.resolve();
    await Promise.resolve();
    state.inFlight -= 1;
    return index;
  });
  return { tasks, state };
}

describe("ex040 runSequential", () => {
  it("returns the results in order", async () => {
    const { tasks } = tracked(3);
    await expect(runSequential(tasks)).resolves.toEqual([0, 1, 2]);
  });

  it("keeps exactly one task in flight", async () => {
    const { tasks, state } = tracked(4);
    await runSequential(tasks);
    expect(state.peak).toBe(1);
  });

  it("stops at the first failure", async () => {
    const later = vi.fn();
    await expect(
      runSequential([
        async () => 1,
        async () => {
          throw new Error("boom");
        },
        later,
      ]),
    ).rejects.toThrow("boom");
    expect(later).not.toHaveBeenCalled();
  });

  it("handles no tasks", async () => {
    await expect(runSequential([])).resolves.toEqual([]);
  });
});

describe("ex040 runParallel", () => {
  it("returns the results in INPUT order", async () => {
    const { tasks } = tracked(3);
    await expect(runParallel(tasks)).resolves.toEqual([0, 1, 2]);
  });

  it("really overlaps them", async () => {
    // The fact a sequential implementation cannot fake.
    const { tasks, state } = tracked(4);
    await runParallel(tasks);
    expect(state.peak).toBe(4);
  });

  it("starts the later tasks even though an earlier one fails", async () => {
    const late = vi.fn(async () => "late");
    await expect(
      runParallel([
        async () => {
          throw new Error("boom");
        },
        late,
      ]),
    ).rejects.toThrow("boom");
    expect(late).toHaveBeenCalledTimes(1);
  });
});

describe("ex040 runParallelIgnoringFailures", () => {
  it("keeps the successes in input order", async () => {
    const result = await runParallelIgnoringFailures([
      async () => "a",
      async () => {
        throw new Error("x");
      },
      async () => "b",
    ]);
    expect(result).toEqual(["a", "b"]);
  });

  it("overlaps them too", async () => {
    const { tasks, state } = tracked(3);
    await runParallelIgnoringFailures(tasks);
    expect(state.peak).toBe(3);
  });

  it("returns an empty array when everything fails", async () => {
    await expect(
      runParallelIgnoringFailures([
        async () => {
          throw new Error("x");
        },
      ]),
    ).resolves.toEqual([]);
  });
});

describe("ex040 runBoth", () => {
  it("names the results", async () => {
    await expect(runBoth(async () => 1, async () => 2)).resolves.toEqual({
      first: 1,
      second: 2,
    });
  });

  it("starts the second task before awaiting the first", async () => {
    const log = [];
    const slowFirst = async () => {
      log.push("first started");
      await Promise.resolve();
      await Promise.resolve();
      log.push("first done");
      return 1;
    };
    const second = async () => {
      log.push("second started");
      return 2;
    };
    await runBoth(slowFirst, second);
    expect(log).toEqual(["first started", "second started", "first done"]);
  });
});

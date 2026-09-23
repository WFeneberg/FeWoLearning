import { describe, expect, it } from "vitest";
import { runGroup } from "@ex/03-advanced/ex087_task_group/index";

/** A promise plus the handle to settle it, so the test decides timing. */
function deferred<T>(): {
  promise: Promise<T>;
  resolve: (value: T) => void;
  reject: (error: unknown) => void;
} {
  let resolve!: (value: T) => void;
  let reject!: (error: unknown) => void;
  const promise = new Promise<T>((res, rej) => {
    resolve = res;
    reject = rej;
  });
  return { promise, resolve, reject };
}

describe("ex087 runGroup", () => {
  it("collects every value when nothing fails", async () => {
    const result = await runGroup([async () => 1, async () => 2, async () => 3]);
    expect(result).toEqual({ values: [1, 2, 3], failure: undefined });
  });

  it("keeps task order, not completion order", async () => {
    const first = deferred<number>();
    const second = deferred<number>();
    const pending = runGroup([async () => first.promise, async () => second.promise]);
    second.resolve(2);
    first.resolve(1);
    await expect(pending).resolves.toEqual({ values: [1, 2], failure: undefined });
  });

  it("handles no tasks at all", async () => {
    await expect(runGroup([])).resolves.toEqual({ values: [], failure: undefined });
  });

  // Concurrency itself: no fact about RESULTS can tell a parallel group
  // from a serial one, so this observes that all three have started
  // before any is allowed to finish.
  it("starts every task before any of them finishes", async () => {
    const started: number[] = [];
    const gate = deferred<void>();
    const pending = runGroup(
      [0, 1, 2].map((index) => async () => {
        started.push(index);
        await gate.promise;
        return index;
      }),
    );
    pending.catch(() => undefined);
    await Promise.resolve();
    expect(started).toEqual([0, 1, 2]);
    gate.resolve();
    await pending.catch(() => undefined);
  });

  it("reports the first failure's message", async () => {
    const result = await runGroup([
      async () => 1,
      async () => {
        throw new Error("down");
      },
    ]);
    expect(result.failure).toBe("down");
  });

  it("leaves undefined where a task failed", async () => {
    const result = await runGroup<number>([
      async () => 1,
      async () => {
        throw new Error("down");
      },
      async () => 3,
    ]);
    expect(result.values).toEqual([1, undefined, 3]);
  });

  // The signal is the group's, and every task gets the same one.
  it("hands every task the same signal, unaborted at the start", async () => {
    const seen: AbortSignal[] = [];
    await runGroup([
      async (signal) => {
        seen.push(signal);
        return 1;
      },
      async (signal) => {
        seen.push(signal);
        return 2;
      },
    ]);
    expect(seen).toHaveLength(2);
    expect(seen[0]).toBe(seen[1]);
    expect(seen[0]?.aborted).toBe(false);
  });

  // The point of the whole row: a sibling learns about the failure.
  it("aborts the siblings when one task fails", async () => {
    let siblingSawAbort = false;
    const failed = deferred<number>();
    const result = await runGroup<number>([
      async (signal) => {
        await failed.promise.catch(() => undefined);
        siblingSawAbort = signal.aborted;
        return 1;
      },
      async () => {
        failed.reject(new Error("down"));
        throw new Error("down");
      },
    ]);
    expect(result.failure).toBe("down");
    expect(siblingSawAbort).toBe(true);
  });

  // And it waits: the slow sibling has finished by the time we get here.
  it("waits for the siblings before returning", async () => {
    let finished = false;
    const result = await runGroup<number>([
      async () => {
        await Promise.resolve();
        await Promise.resolve();
        finished = true;
        return 1;
      },
      async () => {
        throw new Error("down");
      },
    ]);
    expect(result.failure).toBe("down");
    expect(finished).toBe(true);
  });
});

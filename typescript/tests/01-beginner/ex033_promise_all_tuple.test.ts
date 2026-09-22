import { describe, expect, it } from "vitest";
import { loadAll, loadAllOrMessage } from "@ex/01-beginner/ex033_promise_all_tuple/index";

/** A promise plus the handles to settle it whenever the test likes. */
function deferred<T>(): { promise: Promise<T>; resolve: (value: T) => void; reject: (error: unknown) => void } {
  let resolve!: (value: T) => void;
  let reject!: (error: unknown) => void;
  const promise = new Promise<T>((res, rej) => {
    resolve = res;
    reject = rej;
  });
  return { promise, resolve, reject };
}

describe("ex033 loadAll", () => {
  it("collects all three values", async () => {
    await expect(
      loadAll(Promise.resolve("a"), Promise.resolve(1), Promise.resolve(true)),
    ).resolves.toEqual(["a", 1, true]);
  });

  // Argument order, not settle order — the third one finishes first here.
  it("keeps argument order whatever the settle order", async () => {
    const a = deferred<string>();
    const b = deferred<number>();
    const c = deferred<boolean>();
    const pending = loadAll(a.promise, b.promise, c.promise);
    c.resolve(true);
    b.resolve(1);
    a.resolve("a");
    await expect(pending).resolves.toEqual(["a", 1, true]);
  });

  it("rejects when any one of them rejects", async () => {
    // The no-op catch is test hygiene, not part of the subject: on the
    // untouched stub loadAll throws before it consumes this promise, and an
    // unconsumed rejection surfaces as an unhandled error that muddies the
    // run. Attaching a handler keeps the rejection observable to the
    // assertion below while claiming it for the runtime.
    const failing: Promise<number> = Promise.reject(new Error("down"));
    failing.catch(() => undefined);
    await expect(
      loadAll(Promise.resolve("a"), failing, Promise.resolve(true)),
    ).rejects.toThrow("down");
  });
});

describe("ex033 loadAllOrMessage", () => {
  it("returns the values when all succeed", async () => {
    await expect(
      loadAllOrMessage([Promise.resolve(1), Promise.resolve(2)]),
    ).resolves.toEqual([1, 2]);
  });

  it("returns an empty list for no promises", async () => {
    await expect(loadAllOrMessage([])).resolves.toEqual([]);
  });

  // Fail-fast: the message comes back before the slow one has settled.
  it("reports the first rejection without waiting for the rest", async () => {
    const slow = deferred<number>();
    const failing: Promise<number> = Promise.reject(new Error("down"));
    failing.catch(() => undefined);
    const result = await loadAllOrMessage([slow.promise, failing]);
    expect(result).toBe("failed:down");
    slow.resolve(1);
  });
});

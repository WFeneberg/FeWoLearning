import { describe, expect, it } from "vitest";
import { runSequentially, withAbort } from "@ex/02-intermediate/ex058_abort_signal/index";

function deferred<T>(): { promise: Promise<T>; resolve: (value: T) => void } {
  let resolve!: (value: T) => void;
  const promise = new Promise<T>((res) => {
    resolve = res;
  });
  return { promise, resolve };
}

describe("ex058 withAbort", () => {
  it("resolves with the work when nothing aborts", async () => {
    const controller = new AbortController();
    await expect(withAbort(Promise.resolve(42), controller.signal)).resolves.toBe(42);
  });

  it("rejects with the reason when the signal fires first", async () => {
    const controller = new AbortController();
    const never = deferred<number>();
    const pending = withAbort(never.promise, controller.signal);
    controller.abort(new Error("cancelled"));
    await expect(pending).rejects.toThrow("cancelled");
    never.resolve(1);
  });

  // An abort listener never fires for a signal that already aborted, so
  // this is the fact that forces the flag check before the subscription.
  it("rejects immediately for an already-aborted signal", async () => {
    const controller = new AbortController();
    controller.abort(new Error("too late"));
    const never = deferred<number>();
    await expect(withAbort(never.promise, controller.signal)).rejects.toThrow("too late");
    never.resolve(1);
  });

  it("rejects with the default DOMException when abort is given no reason", async () => {
    const controller = new AbortController();
    controller.abort();
    const never = deferred<number>();
    await expect(withAbort(never.promise, controller.signal)).rejects.toMatchObject({
      name: "AbortError",
    });
    never.resolve(1);
  });
});

describe("ex058 runSequentially", () => {
  it("runs everything when nothing aborts", async () => {
    const controller = new AbortController();
    const tasks = [async () => 1, async () => 2, async () => 3];
    await expect(runSequentially(tasks, controller.signal)).resolves.toEqual([1, 2, 3]);
  });

  it("stops before the next task once aborted", async () => {
    const controller = new AbortController();
    const started: number[] = [];
    const tasks = [
      async () => {
        started.push(1);
        return 1;
      },
      async () => {
        started.push(2);
        controller.abort();
        return 2;
      },
      async () => {
        started.push(3);
        return 3;
      },
    ];
    await expect(runSequentially(tasks, controller.signal)).resolves.toEqual([1, 2]);
    expect(started).toEqual([1, 2]);
  });

  it("runs nothing at all for an already-aborted signal", async () => {
    const controller = new AbortController();
    controller.abort();
    const started: number[] = [];
    const tasks = [
      async () => {
        started.push(1);
        return 1;
      },
    ];
    await expect(runSequentially(tasks, controller.signal)).resolves.toEqual([]);
    expect(started).toEqual([]);
  });
});

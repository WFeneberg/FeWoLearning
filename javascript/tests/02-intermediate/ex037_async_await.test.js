import { describe, expect, it, vi } from "vitest";
import {
  awaitPlain,
  failsLater,
  loadProfile,
  safeAwait,
  sumSequential,
} from "@ex/02-intermediate/ex037_async_await/index.js";

describe("ex037 loadProfile", () => {
  const api = () => ({
    getUser: vi.fn(async (id) => ({ id: `user-${id}` })),
    getOrders: vi.fn(async (userId) => [`order for ${userId}`]),
  });

  it("chains the second call onto the first one's result", async () => {
    const fake = api();
    await expect(loadProfile(fake, 7)).resolves.toEqual({
      user: { id: "user-7" },
      orders: ["order for user-7"],
    });
    expect(fake.getOrders).toHaveBeenCalledWith("user-7");
  });

  it("lets a failure in the first call reject the whole thing", async () => {
    const fake = api();
    fake.getUser = vi.fn(async () => {
      throw new Error("no user");
    });
    await expect(loadProfile(fake, 1)).rejects.toThrow("no user");
    expect(fake.getOrders).not.toHaveBeenCalled();
  });
});

describe("ex037 safeAwait", () => {
  it("reports success as [null, value]", async () => {
    await expect(safeAwait(Promise.resolve("ok"))).resolves.toEqual([null, "ok"]);
  });

  it("reports failure as [error, undefined] without rejecting", async () => {
    const error = new Error("boom");
    const [caught, value] = await safeAwait(Promise.reject(error));
    expect(caught).toBe(error);
    expect(value).toBeUndefined();
  });

  it("accepts a plain value too", async () => {
    await expect(safeAwait("plain")).resolves.toEqual([null, "plain"]);
  });
});

describe("ex037 failsLater", () => {
  it("does not throw at the call — it returns a rejected promise", () => {
    // The distinction that matters: this cannot be caught by a try/catch
    // that does not await.
    let promise;
    expect(() => {
      promise = failsLater("nope");
    }).not.toThrow();
    expect(promise).toBeInstanceOf(Promise);
    return expect(promise).rejects.toThrow(RangeError);
  });

  it("carries the message", async () => {
    await expect(failsLater("specific")).rejects.toThrow("specific");
  });
});

describe("ex037 awaitPlain", () => {
  it("returns the value, promise or not", async () => {
    await expect(awaitPlain(5, [])).resolves.toBe(5);
    await expect(awaitPlain(Promise.resolve(5), [])).resolves.toBe(5);
  });

  it("suspends at the await even for a non-promise", async () => {
    // The body runs up to the await synchronously; everything after it is a
    // microtask. So the caller sees ["before"] before it awaits.
    const log = [];
    const promise = awaitPlain("x", log);
    expect(log).toEqual(["before"]);
    await promise;
    expect(log).toEqual(["before", "after"]);
  });
});

describe("ex037 sumSequential", () => {
  it("sums", async () => {
    const api = { getValue: async (id) => id * 10 };
    await expect(sumSequential(api, [1, 2, 3])).resolves.toBe(60);
    await expect(sumSequential(api, [])).resolves.toBe(0);
  });

  it("never has two calls in flight at once", async () => {
    let inFlight = 0;
    let peak = 0;
    const api = {
      async getValue(id) {
        peak = Math.max(peak, ++inFlight);
        await Promise.resolve();
        inFlight--;
        return id;
      },
    };
    await sumSequential(api, [1, 2, 3, 4]);
    expect(peak).toBe(1);
  });
});

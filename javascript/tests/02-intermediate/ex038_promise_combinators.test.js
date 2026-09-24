import { describe, expect, it } from "vitest";
import {
  collectAll,
  collectSettled,
  firstSettled,
  firstSuccess,
} from "@ex/02-intermediate/ex038_promise_combinators/index.js";

/** A rejected promise nobody consumed is an unhandled rejection; claim it. */
const rejected = (reason) => {
  const promise = Promise.reject(reason);
  promise.catch(() => undefined);
  return promise;
};

describe("ex038 collectAll", () => {
  it("keeps input order, not settle order", async () => {
    const slow = new Promise((resolve) => queueMicrotask(() => resolve("slow")));
    await expect(collectAll([slow, Promise.resolve("fast")])).resolves.toEqual(["slow", "fast"]);
  });

  it("accepts plain values alongside promises", async () => {
    await expect(collectAll([1, Promise.resolve(2)])).resolves.toEqual([1, 2]);
  });

  it("rejects with the first rejection", async () => {
    await expect(
      collectAll([Promise.resolve(1), rejected(new Error("first")), rejected(new Error("second"))]),
    ).rejects.toThrow("first");
  });

  it("resolves to an empty array for no promises", async () => {
    await expect(collectAll([])).resolves.toEqual([]);
  });
});

describe("ex038 collectSettled", () => {
  it("reports both outcomes without rejecting", async () => {
    const result = await collectSettled([
      Promise.resolve("a"),
      rejected(new Error("bad")),
      Promise.resolve("b"),
    ]);
    expect(result.fulfilled).toEqual(["a", "b"]);
    expect(result.rejected).toHaveLength(1);
    expect(result.rejected[0].message).toBe("bad");
  });

  it("survives every promise failing", async () => {
    const result = await collectSettled([rejected(1), rejected(2)]);
    expect(result.fulfilled).toEqual([]);
    expect(result.rejected).toEqual([1, 2]);
  });

  it("handles an empty list", async () => {
    await expect(collectSettled([])).resolves.toEqual({ fulfilled: [], rejected: [] });
  });
});

describe("ex038 firstSettled", () => {
  it("takes the first value", async () => {
    const pending = new Promise(() => {});
    await expect(firstSettled([pending, Promise.resolve("quick")])).resolves.toBe("quick");
  });

  it("lets a rejection win the race", async () => {
    // race settles on the first SETTLEMENT, which is the difference from any().
    const pending = new Promise(() => {});
    await expect(firstSettled([rejected(new Error("early failure")), pending])).rejects.toThrow(
      "early failure",
    );
  });
});

describe("ex038 firstSuccess", () => {
  it("skips rejections and takes the first value", async () => {
    await expect(
      firstSuccess([rejected(new Error("nope")), Promise.resolve("winner")]),
    ).resolves.toBe("winner");
  });

  it("rejects with an AggregateError only when everything fails", async () => {
    try {
      await firstSuccess([rejected(new Error("a")), rejected(new Error("b"))]);
      expect.unreachable("should have rejected");
    } catch (error) {
      expect(error).toBeInstanceOf(AggregateError);
      expect(error.errors.map((e) => e.message)).toEqual(["a", "b"]);
    }
  });

  it("rejects with an AggregateError for an empty list", async () => {
    await expect(firstSuccess([])).rejects.toBeInstanceOf(AggregateError);
  });
});

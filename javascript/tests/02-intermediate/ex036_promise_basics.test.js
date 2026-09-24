import { describe, expect, it, vi } from "vitest";
import {
  deferred,
  mapResolved,
  runFinally,
  settleOnce,
  withFallback,
} from "@ex/02-intermediate/ex036_promise_basics/index.js";

describe("ex036 deferred", () => {
  it("resolves from outside", async () => {
    const { promise, resolve } = deferred();
    resolve(42);
    await expect(promise).resolves.toBe(42);
  });

  it("rejects from outside", async () => {
    const { promise, reject } = deferred();
    reject(new Error("nope"));
    await expect(promise).rejects.toThrow("nope");
  });

  it("hands out both functions immediately, not on a later tick", () => {
    const { resolve, reject } = deferred();
    expect(typeof resolve).toBe("function");
    expect(typeof reject).toBe("function");
  });

  it("is still pending until it is told otherwise", async () => {
    const { promise } = deferred();
    const raced = await Promise.race([promise, Promise.resolve("pending")]);
    expect(raced).toBe("pending");
  });
});

describe("ex036 settleOnce", () => {
  it("keeps the first settlement", async () => {
    await expect(settleOnce()).resolves.toBe("first");
  });

  it("discards the late rejection instead of crashing on it", async () => {
    // A rejection after a resolve is not an unhandled rejection — the
    // promise is already settled and stops listening.
    const promise = settleOnce();
    await expect(promise).resolves.toBe("first");
    await expect(promise).resolves.toBe("first");
  });
});

describe("ex036 mapResolved", () => {
  it("maps the value", async () => {
    await expect(mapResolved(Promise.resolve(2), (n) => n * 3)).resolves.toBe(6);
  });

  it("returns a new promise rather than the original", () => {
    const original = Promise.resolve(1);
    expect(mapResolved(original, (n) => n)).not.toBe(original);
  });

  it("turns a throwing mapper into a rejection", async () => {
    await expect(
      mapResolved(Promise.resolve(1), () => {
        throw new RangeError("bad");
      }),
    ).rejects.toThrow(RangeError);
  });

  it("passes a rejection straight through", async () => {
    const fn = vi.fn();
    await expect(mapResolved(Promise.reject(new Error("upstream")), fn)).rejects.toThrow(
      "upstream",
    );
    expect(fn).not.toHaveBeenCalled();
  });
});

describe("ex036 withFallback", () => {
  it("uses the fallback on rejection", async () => {
    await expect(withFallback(Promise.reject(new Error("x")), "fb")).resolves.toBe("fb");
  });

  it("leaves a fulfilled value alone", async () => {
    await expect(withFallback(Promise.resolve("ok"), "fb")).resolves.toBe("ok");
  });
});

describe("ex036 runFinally", () => {
  it("runs the callback and keeps the value", async () => {
    const onDone = vi.fn();
    await expect(runFinally(Promise.resolve("value"), onDone)).resolves.toBe("value");
    expect(onDone).toHaveBeenCalledTimes(1);
  });

  it("runs the callback and keeps the rejection", async () => {
    const onDone = vi.fn();
    await expect(runFinally(Promise.reject(new Error("boom")), onDone)).rejects.toThrow("boom");
    expect(onDone).toHaveBeenCalledTimes(1);
  });

  it("ignores what the callback returns", async () => {
    // .finally() is not .then(): its return value is discarded.
    await expect(runFinally(Promise.resolve("value"), () => "replacement")).resolves.toBe(
      "value",
    );
  });
});

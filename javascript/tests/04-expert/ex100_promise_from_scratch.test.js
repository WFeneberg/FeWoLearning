import { describe, expect, it, vi } from "vitest";
import { Thenable } from "@ex/04-expert/ex100_promise_from_scratch/index.js";

/** Lets a test await a Thenable without relying on await's own adoption. */
const settled = (thenable) =>
  new Promise((resolve) =>
    thenable.then(
      (value) => resolve({ status: "fulfilled", value }),
      (reason) => resolve({ status: "rejected", reason }),
    ),
  );

describe("ex100 construction and settling", () => {
  it("runs the executor synchronously", () => {
    const ran = vi.fn();
    new Thenable(ran);
    expect(ran).toHaveBeenCalledTimes(1);
  });

  it("fulfils", async () => {
    const promise = new Thenable((resolve) => resolve(42));
    await expect(settled(promise)).resolves.toEqual({ status: "fulfilled", value: 42 });
  });

  it("rejects", async () => {
    const error = new Error("nope");
    const promise = new Thenable((_resolve, reject) => reject(error));
    await expect(settled(promise)).resolves.toEqual({ status: "rejected", reason: error });
  });

  it("rejects when the executor throws", async () => {
    const promise = new Thenable(() => {
      throw new Error("executor failed");
    });
    const outcome = await settled(promise);
    expect(outcome.status).toBe("rejected");
    expect(outcome.reason.message).toBe("executor failed");
  });

  it("settles once and ignores everything after", async () => {
    const promise = new Thenable((resolve, reject) => {
      resolve("first");
      resolve("second");
      reject(new Error("too late"));
    });
    await expect(settled(promise)).resolves.toEqual({ status: "fulfilled", value: "first" });
  });

  it("settles asynchronously when the executor does", async () => {
    const promise = new Thenable((resolve) => queueMicrotask(() => resolve("later")));
    await expect(settled(promise)).resolves.toEqual({ status: "fulfilled", value: "later" });
  });
});

describe("ex100 then", () => {
  it("never runs a handler synchronously", async () => {
    // Even for an already-settled promise: a handler is a microtask, which
    // is the rule that makes ordering predictable at all.
    const order = [];
    const promise = new Thenable((resolve) => resolve(1));
    promise.then(() => order.push("handler"));
    order.push("sync");
    await settled(promise);
    expect(order).toEqual(["sync", "handler"]);
  });

  it("returns a new promise", () => {
    const promise = new Thenable((resolve) => resolve(1));
    expect(promise.then(() => {})).toBeInstanceOf(Thenable);
    expect(promise.then(() => {})).not.toBe(promise);
  });

  it("chains a handler's return value", async () => {
    const chained = new Thenable((resolve) => resolve(1)).then((n) => n + 1).then((n) => n * 10);
    await expect(settled(chained)).resolves.toEqual({ status: "fulfilled", value: 20 });
  });

  it("rejects the chain when a handler throws", async () => {
    const chained = new Thenable((resolve) => resolve(1)).then(() => {
      throw new Error("handler failed");
    });
    const outcome = await settled(chained);
    expect(outcome.status).toBe("rejected");
    expect(outcome.reason.message).toBe("handler failed");
  });

  it("passes a value through a missing handler", async () => {
    const chained = new Thenable((resolve) => resolve("value")).then(null).then(undefined);
    await expect(settled(chained)).resolves.toEqual({ status: "fulfilled", value: "value" });
  });

  it("passes a rejection through a fulfil-only handler", async () => {
    const onFulfilled = vi.fn();
    const chained = new Thenable((_resolve, reject) => reject("reason")).then(onFulfilled);
    await expect(settled(chained)).resolves.toEqual({ status: "rejected", reason: "reason" });
    expect(onFulfilled).not.toHaveBeenCalled();
  });

  it("recovers from a rejection through onRejected", async () => {
    const chained = new Thenable((_resolve, reject) => reject(new Error("x"))).then(
      undefined,
      () => "recovered",
    );
    await expect(settled(chained)).resolves.toEqual({ status: "fulfilled", value: "recovered" });
  });

  it("calls every handler registered on the same promise", async () => {
    const promise = new Thenable((resolve) => resolve("v"));
    const seen = [];
    promise.then((v) => seen.push(`a:${v}`));
    promise.then((v) => seen.push(`b:${v}`));
    await settled(promise);
    expect(seen).toEqual(["a:v", "b:v"]);
  });

  it("adopts a Thenable a handler returns instead of nesting it", async () => {
    const chained = new Thenable((resolve) => resolve(1)).then(
      () => new Thenable((resolve) => queueMicrotask(() => resolve("inner"))),
    );
    await expect(settled(chained)).resolves.toEqual({ status: "fulfilled", value: "inner" });
  });

  it("adopts a native promise too", async () => {
    const chained = new Thenable((resolve) => resolve(1)).then(() => Promise.resolve("native"));
    await expect(settled(chained)).resolves.toEqual({ status: "fulfilled", value: "native" });
  });

  it("adopts a rejected thenable as a rejection", async () => {
    const chained = new Thenable((resolve) => resolve(1)).then(() =>
      Thenable.reject(new Error("inner failure")),
    );
    const outcome = await settled(chained);
    expect(outcome.status).toBe("rejected");
    expect(outcome.reason.message).toBe("inner failure");
  });
});

describe("ex100 catch and finally", () => {
  it("catches", async () => {
    const chained = Thenable.reject(new Error("x")).catch((error) => `caught ${error.message}`);
    await expect(settled(chained)).resolves.toEqual({ status: "fulfilled", value: "caught x" });
  });

  it("runs finally on both paths and keeps the outcome", async () => {
    const onFinally = vi.fn();
    await expect(settled(Thenable.resolve("v").finally(onFinally))).resolves.toEqual({
      status: "fulfilled",
      value: "v",
    });
    const rejected = await settled(Thenable.reject("r").finally(onFinally));
    expect(rejected).toEqual({ status: "rejected", reason: "r" });
    expect(onFinally).toHaveBeenCalledTimes(2);
  });

  it("discards what finally returns", async () => {
    await expect(settled(Thenable.resolve("v").finally(() => "replacement"))).resolves.toEqual({
      status: "fulfilled",
      value: "v",
    });
  });
});

describe("ex100 statics", () => {
  it("resolve wraps a value and passes a Thenable through", async () => {
    await expect(settled(Thenable.resolve(1))).resolves.toEqual({
      status: "fulfilled",
      value: 1,
    });
    const existing = Thenable.resolve(1);
    expect(Thenable.resolve(existing)).toBe(existing);
  });

  it("resolve adopts a native promise", async () => {
    await expect(settled(Thenable.resolve(Promise.resolve("native")))).resolves.toEqual({
      status: "fulfilled",
      value: "native",
    });
  });

  it("all keeps input order", async () => {
    const slow = new Thenable((resolve) => queueMicrotask(() => resolve("slow")));
    await expect(settled(Thenable.all([slow, Thenable.resolve("fast"), "plain"]))).resolves.toEqual(
      { status: "fulfilled", value: ["slow", "fast", "plain"] },
    );
  });

  it("all rejects with the first rejection", async () => {
    const outcome = await settled(
      Thenable.all([Thenable.resolve(1), Thenable.reject(new Error("first"))]),
    );
    expect(outcome.status).toBe("rejected");
    expect(outcome.reason.message).toBe("first");
  });

  it("all resolves to [] for nothing", async () => {
    await expect(settled(Thenable.all([]))).resolves.toEqual({ status: "fulfilled", value: [] });
  });
});

describe("ex100 interoperability", () => {
  it("can be awaited, because await only needs a then method", async () => {
    await expect(new Thenable((resolve) => resolve("awaited"))).resolves.toBe("awaited");
  });

  it("can be adopted by a native promise chain", async () => {
    await expect(Promise.resolve().then(() => Thenable.resolve("adopted"))).resolves.toBe(
      "adopted",
    );
  });
});

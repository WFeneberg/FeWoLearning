import { describe, expect, it } from "vitest";
import {
  firstSettled,
  firstSuccess,
  partition,
} from "@ex/02-intermediate/ex056_promise_combinators/index";

function rejected<T>(message: string): Promise<T> {
  const promise: Promise<T> = Promise.reject(new Error(message));
  // Claimed so an implementation that never consumes it does not leave an
  // unhandled rejection behind.
  promise.catch(() => undefined);
  return promise;
}

describe("ex056 partition", () => {
  it("reports both halves", async () => {
    await expect(
      partition([Promise.resolve(1), rejected<number>("down"), Promise.resolve(3)]),
    ).resolves.toEqual({ values: [1, 3], errors: ["down"] });
  });

  it("keeps input order within each half", async () => {
    await expect(
      partition([rejected<number>("a"), Promise.resolve(2), rejected<number>("b")]),
    ).resolves.toEqual({ values: [2], errors: ["a", "b"] });
  });

  it("handles all-success and all-failure", async () => {
    await expect(partition([Promise.resolve(1)])).resolves.toEqual({
      values: [1],
      errors: [],
    });
    await expect(partition([rejected<number>("x")])).resolves.toEqual({
      values: [],
      errors: ["x"],
    });
  });

  it("handles an empty input", async () => {
    await expect(partition([])).resolves.toEqual({ values: [], errors: [] });
  });
});

describe("ex056 firstSettled", () => {
  it("takes the first value", async () => {
    await expect(firstSettled([Promise.resolve("a")])).resolves.toBe("ok:a");
  });

  // race settles on a REJECTION too, which is the whole difference from any.
  it("loses to a rejection that settles first", async () => {
    await expect(firstSettled([rejected<string>("down"), Promise.resolve("a")])).resolves.toBe(
      "fail:down",
    );
  });
});

describe("ex056 firstSuccess", () => {
  it("skips a rejection and takes the first value", async () => {
    await expect(
      firstSuccess([rejected<string>("down"), Promise.resolve("a")]),
    ).resolves.toBe("a");
  });

  it("takes the only value there is", async () => {
    await expect(firstSuccess([Promise.resolve("a")])).resolves.toBe("a");
  });
});

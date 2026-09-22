import { describe, expect, it } from "vitest";
import {
  firstSuccessOrSummary,
  reasonsOf,
} from "@ex/02-intermediate/ex057_aggregate_error/index";

function rejected<T>(message: string): Promise<T> {
  const promise: Promise<T> = Promise.reject(new Error(message));
  promise.catch(() => undefined);
  return promise;
}

describe("ex057 reasonsOf", () => {
  it("reads the messages out of an AggregateError", () => {
    const error = new AggregateError([new Error("a"), new Error("b")], "all failed");
    expect(reasonsOf(error)).toEqual(["a", "b"]);
  });

  it("stringifies an entry that is not an Error", () => {
    expect(reasonsOf(new AggregateError(["plain", 42], "x"))).toEqual(["plain", "42"]);
  });

  it("returns nothing for an ordinary Error", () => {
    expect(reasonsOf(new Error("boom"))).toEqual([]);
  });

  it("returns nothing for a non-error", () => {
    expect(reasonsOf("boom")).toEqual([]);
    expect(reasonsOf(null)).toEqual([]);
  });
});

describe("ex057 firstSuccessOrSummary", () => {
  it("returns the first success", async () => {
    await expect(
      firstSuccessOrSummary([rejected<string>("a"), Promise.resolve("ok")]),
    ).resolves.toBe("ok");
  });

  it("summarises every reason when all fail, in input order", async () => {
    await expect(
      firstSuccessOrSummary([rejected<string>("a"), rejected<string>("b")]),
    ).resolves.toBe("none:a|b");
  });

  // Promise.any([]) rejects immediately — the opposite of Promise.all([]).
  it("treats no promises at all as a failure", async () => {
    await expect(firstSuccessOrSummary([])).resolves.toBe("none:");
  });
});

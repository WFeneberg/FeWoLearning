import { describe, expect, it } from "vitest";
import {
  asyncTake,
  collect,
  fromPromises,
  makeAsyncRange,
} from "@ex/02-intermediate/ex041_async_iteration/index.js";

/** An endless async source that records its own cleanup. */
function endless(log) {
  return (async function* source() {
    try {
      let n = 0;
      while (true) {
        log.push(`produced ${n}`);
        yield n++;
      }
    } finally {
      log.push("cleaned up");
    }
  })();
}

describe("ex041 fromPromises", () => {
  it("yields the awaited values in order", async () => {
    const values = [];
    for await (const value of fromPromises([Promise.resolve(1), Promise.resolve(2)])) {
      values.push(value);
    }
    expect(values).toEqual([1, 2]);
  });

  it("propagates a rejection as a throw at the consumer", async () => {
    const iterator = fromPromises([Promise.reject(new Error("bad"))]);
    await expect(iterator.next()).rejects.toThrow("bad");
  });

  it("is async-iterable, not plain-iterable", async () => {
    // Anchored: an `async function*` stub already answers this.
    await expect(fromPromises([Promise.resolve(1)]).next()).resolves.toEqual({
      value: 1,
      done: false,
    });
    const iterator = fromPromises([]);
    expect(typeof iterator[Symbol.asyncIterator]).toBe("function");
    expect(iterator[Symbol.iterator]).toBeUndefined();
  });
});

describe("ex041 collect", () => {
  it("drains an async generator", async () => {
    await expect(collect(fromPromises([Promise.resolve("a"), Promise.resolve("b")]))).resolves.toEqual(
      ["a", "b"],
    );
  });

  it("drains a hand-written async iterable", async () => {
    await expect(collect(makeAsyncRange(3))).resolves.toEqual([0, 1, 2]);
  });

  it("returns an empty array for an empty source", async () => {
    await expect(collect(makeAsyncRange(0))).resolves.toEqual([]);
  });
});

describe("ex041 asyncTake", () => {
  it("takes what was asked for", async () => {
    await expect(collect(asyncTake(makeAsyncRange(10), 3))).resolves.toEqual([0, 1, 2]);
    await expect(collect(asyncTake(makeAsyncRange(2), 9))).resolves.toEqual([0, 1]);
    await expect(collect(asyncTake(makeAsyncRange(9), 0))).resolves.toEqual([]);
  });

  it("stops pulling an endless source", async () => {
    const log = [];
    await expect(collect(asyncTake(endless(log), 3))).resolves.toEqual([0, 1, 2]);
    // One produced value per taken value, and no fourth.
    expect(log.filter((entry) => entry.startsWith("produced"))).toEqual([
      "produced 0",
      "produced 1",
      "produced 2",
    ]);
  });

  it("lets the source clean up, because breaking calls its return()", async () => {
    const log = [];
    await collect(asyncTake(endless(log), 2));
    expect(log.at(-1)).toBe("cleaned up");
  });
});

describe("ex041 makeAsyncRange", () => {
  it("counts up to the limit", async () => {
    const values = [];
    for await (const value of makeAsyncRange(4)) values.push(value);
    expect(values).toEqual([0, 1, 2, 3]);
  });

  it("hands out a fresh iterator, so it can be walked twice", async () => {
    const range = makeAsyncRange(2);
    await expect(collect(range)).resolves.toEqual([0, 1]);
    await expect(collect(range)).resolves.toEqual([0, 1]);
  });

  it("is async-iterable only", () => {
    expect(typeof makeAsyncRange(1)[Symbol.asyncIterator]).toBe("function");
    expect(makeAsyncRange(1)[Symbol.iterator]).toBeUndefined();
  });
});

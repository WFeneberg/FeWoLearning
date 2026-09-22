import { describe, expect, it } from "vitest";
import { collect, filterAsync, inOrder } from "@ex/02-intermediate/ex060_async_generators/index";

function deferred<T>(): { promise: Promise<T>; resolve: (value: T) => void } {
  let resolve!: (value: T) => void;
  const promise = new Promise<T>((res) => {
    resolve = res;
  });
  return { promise, resolve };
}

/** A hand-written async iterable, to prove the helpers take the protocol
 *  rather than one implementation. */
function fromArray<T>(items: readonly T[]): AsyncIterable<T> {
  return {
    async *[Symbol.asyncIterator]() {
      for (const item of items) {
        yield await Promise.resolve(item);
      }
    },
  };
}

describe("ex060 inOrder", () => {
  it("yields in argument order", async () => {
    const seen: number[] = [];
    for await (const value of inOrder([Promise.resolve(1), Promise.resolve(2)])) {
      seen.push(value as number);
    }
    expect(seen).toEqual([1, 2]);
  });

  // The ordering guarantee: the second promise settles first and still
  // waits its turn.
  it("keeps order even when a later promise settles first", async () => {
    const first = deferred<number>();
    const second = deferred<number>();
    const pending = collect(inOrder([first.promise, second.promise]));
    second.resolve(2);
    first.resolve(1);
    await expect(pending).resolves.toEqual([1, 2]);
  });

  it("yields nothing for no promises", async () => {
    await expect(collect(inOrder([]))).resolves.toEqual([]);
  });
});

describe("ex060 collect", () => {
  it("drains a hand-written async iterable", async () => {
    await expect(collect(fromArray(["a", "b"]))).resolves.toEqual(["a", "b"]);
  });

  it("drains an empty one", async () => {
    await expect(collect(fromArray([]))).resolves.toEqual([]);
  });
});

describe("ex060 filterAsync", () => {
  it("keeps only what the predicate accepts, in order", async () => {
    const kept = collect(filterAsync(fromArray([1, 2, 3, 4]), ((n: number) => n % 2 === 0) as never));
    await expect(kept).resolves.toEqual([2, 4]);
  });

  it("can keep nothing", async () => {
    const kept = collect(filterAsync(fromArray([1, 3]), ((n: number) => n % 2 === 0) as never));
    await expect(kept).resolves.toEqual([]);
  });

  it("composes with inOrder", async () => {
    const source = inOrder([Promise.resolve(1), Promise.resolve(2), Promise.resolve(3)]);
    const kept = collect(filterAsync(source, ((n: number) => n > 1) as never));
    await expect(kept).resolves.toEqual([2, 3]);
  });
});

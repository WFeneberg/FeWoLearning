import { describe, expect, it } from "vitest";
import {
  collectAsync,
  mapAsync,
  naturalsWithCleanup,
  takeAsync,
} from "@ex/03-advanced/ex086_async_pipeline/index";

/** A finite async source, so the composition facts have something safe. */
async function* from(items: readonly number[]): AsyncGenerator<number, void, void> {
  for (const item of items) {
    yield await Promise.resolve(item);
  }
}

describe("ex086 operators", () => {
  it("maps every item", async () => {
    const mapped = mapAsync(from([1, 2, 3]), ((n: number) => n * 2) as never);
    await expect(collectAsync(mapped)).resolves.toEqual([2, 4, 6]);
  });

  it("takes a prefix", async () => {
    await expect(collectAsync(takeAsync(from([1, 2, 3, 4]), 2))).resolves.toEqual([1, 2]);
  });

  it("takes everything when asked for more than there is", async () => {
    await expect(collectAsync(takeAsync(from([1, 2]), 9))).resolves.toEqual([1, 2]);
  });

  it("takes nothing for a count of zero", async () => {
    await expect(collectAsync(takeAsync(from([1, 2]), 0))).resolves.toEqual([]);
  });

  it("composes in either order", async () => {
    const mapThenTake = takeAsync(mapAsync(from([1, 2, 3, 4]), ((n: number) => n * 10) as never), 2);
    await expect(collectAsync(mapThenTake)).resolves.toEqual([10, 20]);

    const takeThenMap = mapAsync(takeAsync(from([1, 2, 3, 4]), 2), ((n: number) => n * 10) as never);
    await expect(collectAsync(takeThenMap)).resolves.toEqual([10, 20]);
  });
});

describe("ex086 termination", () => {
  // The row. An operator that drains its source first would never
  // finish here, because the source never ends.
  it("stays lazy enough to take a prefix of an endless source", async () => {
    const source = naturalsWithCleanup(() => undefined);
    await expect(collectAsync(takeAsync(source, 4))).resolves.toEqual([0, 1, 2, 3]);
  });

  it("runs the source's finally when the consumer stops early", async () => {
    let closed = 0;
    const source = naturalsWithCleanup(() => (closed += 1));
    await collectAsync(takeAsync(source, 3));
    expect(closed).toBe(1);
  });

  // Closure propagates up one generator at a time: the take closes the
  // map, and the map closes the source.
  it("propagates the close through an operator in between", async () => {
    let closed = 0;
    const source = naturalsWithCleanup(() => (closed += 1));
    const pipeline = takeAsync(mapAsync(source, ((n: number) => n * 2) as never), 2);
    await expect(collectAsync(pipeline)).resolves.toEqual([0, 2]);
    expect(closed).toBe(1);
  });

  it("runs the finally for a manual break too", async () => {
    let closed = 0;
    for await (const value of naturalsWithCleanup(() => (closed += 1))) {
      if (value === 2) {
        break;
      }
    }
    expect(closed).toBe(1);
  });
});

import { describe, expect, it } from "vitest";
import {
  collectLimit,
  filterStage,
  mapStage,
  pipeline,
  traceStage,
} from "@ex/04-expert/ex095_async_pipeline/index.js";

/** An endless async source that logs what it produced and its cleanup. */
function counting(log) {
  return (async function* source() {
    try {
      let n = 0;
      while (true) {
        log.push(`src:${n}`);
        yield n++;
      }
    } finally {
      log.push("src:end");
    }
  })();
}

const finite = async function* finiteSource(values) {
  for (const value of values) yield value;
};

describe("ex095 stages", () => {
  it("maps", async () => {
    const piped = pipeline(finite([1, 2, 3]), mapStage((n) => n * 2));
    await expect(collectLimit(piped, 10)).resolves.toEqual([2, 4, 6]);
  });

  it("maps with an async function", async () => {
    const piped = pipeline(finite([1, 2]), mapStage(async (n) => n + 1));
    await expect(collectLimit(piped, 10)).resolves.toEqual([2, 3]);
  });

  it("filters", async () => {
    const piped = pipeline(finite([1, 2, 3, 4]), filterStage((n) => n % 2 === 0));
    await expect(collectLimit(piped, 10)).resolves.toEqual([2, 4]);
  });

  it("composes several stages in order", async () => {
    const piped = pipeline(
      finite([1, 2, 3, 4]),
      mapStage((n) => n * 10),
      filterStage((n) => n > 15),
    );
    await expect(collectLimit(piped, 10)).resolves.toEqual([20, 30, 40]);
  });

  it("is the source itself with no stages", async () => {
    const source = finite([1]);
    expect(pipeline(source)).toBe(source);
  });
});

describe("ex095 collectLimit", () => {
  it("stops at the limit", async () => {
    await expect(collectLimit(finite([1, 2, 3]), 2)).resolves.toEqual([1, 2]);
    await expect(collectLimit(finite([1]), 5)).resolves.toEqual([1]);
    await expect(collectLimit(finite([1]), 0)).resolves.toEqual([]);
  });

  it("does not drain an endless source", async () => {
    const log = [];
    await expect(collectLimit(counting(log), 3)).resolves.toEqual([0, 1, 2]);
    expect(log.filter((entry) => entry.startsWith("src:")).at(-1)).toBe("src:end");
  });

  it("pulls exactly as many values as it keeps", async () => {
    const log = [];
    await collectLimit(counting(log), 2);
    expect(log.filter((entry) => /^src:\d+$/.test(entry))).toEqual(["src:0", "src:1"]);
  });
});

describe("ex095 teardown", () => {
  it("starts the stages from the consumer end", async () => {
    // The last stage's body is what the consumer pulls, so it runs first
    // and only then asks its upstream for anything.
    const log = [];
    const piped = pipeline(counting(log), traceStage("a", log), traceStage("b", log));
    await collectLimit(piped, 1);
    expect(log.slice(0, 2)).toEqual(["b:start", "a:start"]);
  });

  it("closes the whole chain on an early exit, source first", async () => {
    // Measured: each stage's return() awaits its upstream before running
    // its own finally, so the source unwinds first and the last stage last.
    const log = [];
    const piped = pipeline(counting(log), traceStage("a", log), traceStage("b", log));
    await collectLimit(piped, 2);
    expect(log.filter((entry) => entry.endsWith(":end"))).toEqual([
      "src:end",
      "a:end",
      "b:end",
    ]);
  });

  it("passes every value through each stage", async () => {
    const log = [];
    const piped = pipeline(finite([1, 2]), traceStage("a", log), traceStage("b", log));
    await collectLimit(piped, 10);
    expect(log.filter((entry) => /:\d+$/.test(entry))).toEqual(["a:1", "b:1", "a:2", "b:2"]);
  });

  it("closes the chain when the source ends on its own too", async () => {
    const log = [];
    await collectLimit(pipeline(finite([1]), traceStage("a", log)), 10);
    expect(log.at(-1)).toBe("a:end");
  });
});

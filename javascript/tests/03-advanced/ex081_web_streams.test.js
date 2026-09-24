import { describe, expect, it } from "vitest";
import {
  collectStream,
  mapStream,
  streamFrom,
  takeAndCancel,
} from "@ex/03-advanced/ex081_web_streams/index.js";

/** An endless source that counts its pulls and records a cancellation. */
function endlessStream(state) {
  return new ReadableStream({
    pull(controller) {
      state.pulls += 1;
      controller.enqueue(state.pulls);
    },
    cancel(reason) {
      state.cancelled = true;
      state.reason = reason;
    },
  });
}

describe("ex081 streamFrom / collectStream", () => {
  it("round-trips an array", async () => {
    await expect(collectStream(streamFrom([1, 2, 3]))).resolves.toEqual([1, 2, 3]);
  });

  it("handles an empty source", async () => {
    await expect(collectStream(streamFrom([]))).resolves.toEqual([]);
  });

  it("produces a real ReadableStream", () => {
    expect(streamFrom([1])).toBeInstanceOf(ReadableStream);
  });

  it("closes rather than hanging", async () => {
    const reader = streamFrom([1]).getReader();
    expect(await reader.read()).toEqual({ value: 1, done: false });
    expect(await reader.read()).toEqual({ value: undefined, done: true });
  });
});

describe("ex081 mapStream", () => {
  it("transforms every chunk", async () => {
    const mapped = streamFrom([1, 2, 3]).pipeThrough(mapStream((n) => n * 10));
    await expect(collectStream(mapped)).resolves.toEqual([10, 20, 30]);
  });

  it("can change the chunk type", async () => {
    const mapped = streamFrom([1, 2]).pipeThrough(mapStream(String));
    await expect(collectStream(mapped)).resolves.toEqual(["1", "2"]);
  });

  it("is a real TransformStream, so it chains", async () => {
    const twice = streamFrom([1])
      .pipeThrough(mapStream((n) => n + 1))
      .pipeThrough(mapStream((n) => n * 3));
    await expect(collectStream(twice)).resolves.toEqual([6]);
  });
});

describe("ex081 takeAndCancel", () => {
  it("takes what was asked for", async () => {
    const state = { pulls: 0, cancelled: false };
    await expect(takeAndCancel(endlessStream(state), 3)).resolves.toEqual({
      chunks: [1, 2, 3],
      cancelled: true,
    });
  });

  it("cancels the source", async () => {
    const state = { pulls: 0, cancelled: false };
    await takeAndCancel(endlessStream(state), 2);
    expect(state.cancelled).toBe(true);
  });

  it("does not drain an endless source", async () => {
    // The laziness fact: a pull source is demand-driven, so reading three
    // chunks costs a handful of pulls, not infinitely many. (A stream may
    // read one chunk ahead to keep its queue full.)
    const state = { pulls: 0, cancelled: false };
    await takeAndCancel(endlessStream(state), 3);
    expect(state.pulls).toBeLessThanOrEqual(5);
  });

  it("stops early when the source ends first", async () => {
    const result = await takeAndCancel(streamFrom([1, 2]), 10);
    expect(result.chunks).toEqual([1, 2]);
  });

  it("leaves the stream unusable afterwards, as a cancelled stream is", async () => {
    const stream = streamFrom([1, 2, 3]);
    await takeAndCancel(stream, 1);
    await expect(collectStream(stream)).rejects.toThrow(TypeError);
  });
});

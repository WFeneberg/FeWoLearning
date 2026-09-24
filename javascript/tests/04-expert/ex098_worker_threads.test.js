import { describe, expect, it } from "vitest";
import {
  failInWorker,
  fillInWorker,
  sendFunction,
  sumInWorker,
} from "@ex/04-expert/ex098_worker_threads/index.js";

describe("ex098 sumInWorker", () => {
  it("computes on another thread", async () => {
    await expect(sumInWorker([1, 2, 3])).resolves.toBe(6);
  });

  it("handles an empty list", async () => {
    await expect(sumInWorker([])).resolves.toBe(0);
  });

  it("can be called several times", async () => {
    await expect(Promise.all([sumInWorker([1]), sumInWorker([2]), sumInWorker([3])])).resolves.toEqual(
      [1, 2, 3],
    );
  });
});

describe("ex098 fillInWorker", () => {
  it("gets the filled bytes back", async () => {
    const { filled } = await fillInWorker(4, 7);
    expect([...filled]).toEqual([7, 7, 7, 7]);
  });

  it("detaches the sender's buffer — that is what transferring means", async () => {
    // A copy would leave this side's buffer intact and usable. A transfer
    // moves the memory: byteLength drops to 0 here.
    const { senderDetached } = await fillInWorker(8, 1);
    expect(senderDetached).toBe(true);
  });

  it("works for a zero-length buffer", async () => {
    const { filled } = await fillInWorker(0, 5);
    expect(filled).toHaveLength(0);
  });
});

describe("ex098 failInWorker", () => {
  it("surfaces a worker error as a rejection", async () => {
    await expect(failInWorker("worker exploded")).rejects.toThrow("worker exploded");
  });

  it("does not hang when the worker dies instead of answering", async () => {
    // Listening only for "message" would wait forever here.
    await expect(failInWorker("no reply")).rejects.toThrow(Error);
  });
});

describe("ex098 sendFunction", () => {
  it("cannot send a function across the boundary", async () => {
    // postMessage uses the structured clone algorithm, like ex049's
    // structuredClone — same refusal, same error name.
    await expect(sendFunction()).resolves.toBe("DataCloneError");
  });
});

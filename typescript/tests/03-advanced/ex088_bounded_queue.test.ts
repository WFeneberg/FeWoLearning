import { describe, expect, it } from "vitest";
import { BoundedQueue } from "@ex/03-advanced/ex088_bounded_queue/index";

/** Lets the microtask queue drain, so pending work settles. */
async function tick(): Promise<void> {
  await Promise.resolve();
  await Promise.resolve();
  await Promise.resolve();
}

describe("ex088 within capacity", () => {
  it("accepts a push without waiting", async () => {
    const queue = new BoundedQueue<number>(2);
    await queue.push(1);
    expect(queue.size).toBe(1);
  });

  it("pops in FIFO order", async () => {
    const queue = new BoundedQueue<number>(3);
    await queue.push(1);
    await queue.push(2);
    await queue.push(3);
    expect([await queue.pop(), await queue.pop(), await queue.pop()]).toEqual([1, 2, 3]);
  });

  it("is empty again after draining", async () => {
    const queue = new BoundedQueue<number>(2);
    await queue.push(1);
    await queue.pop();
    expect(queue.size).toBe(0);
  });
});

describe("ex088 backpressure", () => {
  // The row. A push beyond capacity does not resolve until a pop makes
  // room, and nothing is dropped in the meantime.
  it("holds a push once the buffer is full", async () => {
    const queue = new BoundedQueue<number>(1);
    await queue.push(1);
    let settled = false;
    // Claimed: on the untouched stub this rejects, and an unconsumed
    // rejection surfaces as an unhandled error rather than a red fact.
    void queue.push(2).then(() => (settled = true)).catch(() => undefined);
    await tick();
    expect(settled).toBe(false);
    expect(queue.waitingPushes).toBe(1);
  });

  it("lets the blocked push through when a pop makes room", async () => {
    const queue = new BoundedQueue<number>(1);
    await queue.push(1);
    let settled = false;
    const blocked = queue.push(2).then(() => (settled = true)).catch(() => undefined);
    await tick();
    expect(await queue.pop()).toBe(1);
    await blocked;
    expect(settled).toBe(true);
    expect(queue.waitingPushes).toBe(0);
  });

  it("keeps the item the blocked push was carrying", async () => {
    const queue = new BoundedQueue<number>(1);
    await queue.push(1);
    const blocked = queue.push(2).catch(() => undefined);
    await tick();
    expect(await queue.pop()).toBe(1);
    await blocked;
    expect(await queue.pop()).toBe(2);
  });

  // FIFO among waiters: a Set or a single pending slot fails here.
  it("wakes blocked pushes in the order they arrived", async () => {
    const queue = new BoundedQueue<number>(1);
    await queue.push(0);
    const order: number[] = [];
    const a = queue.push(1).then(() => order.push(1)).catch(() => undefined);
    const b = queue.push(2).then(() => order.push(2)).catch(() => undefined);
    await tick();
    expect(queue.waitingPushes).toBe(2);
    await queue.pop();
    await tick();
    await queue.pop();
    await Promise.all([a, b]);
    expect(order).toEqual([1, 2]);
  });
});

describe("ex088 an empty queue", () => {
  it("holds a pop until something arrives", async () => {
    const queue = new BoundedQueue<number>(2);
    let got: number | undefined;
    void queue.pop().then((value) => (got = value)).catch(() => undefined);
    await tick();
    expect(got).toBeUndefined();
    expect(queue.waitingPops).toBe(1);
  });

  it("hands the next push straight to the waiting pop", async () => {
    const queue = new BoundedQueue<number>(2);
    const pending = queue.pop();
    pending.catch(() => undefined);
    await tick();
    await queue.push(7);
    expect(await pending).toBe(7);
    // Handed over, not buffered: doing both would duplicate the item.
    expect(queue.size).toBe(0);
    expect(queue.waitingPops).toBe(0);
  });

  it("wakes blocked pops in the order they arrived", async () => {
    const queue = new BoundedQueue<number>(2);
    const first = queue.pop();
    const second = queue.pop();
    first.catch(() => undefined);
    second.catch(() => undefined);
    await tick();
    expect(queue.waitingPops).toBe(2);
    await queue.push(1);
    await queue.push(2);
    expect([await first, await second]).toEqual([1, 2]);
  });
});

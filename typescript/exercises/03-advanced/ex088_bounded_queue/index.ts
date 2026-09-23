// Exercise 088 — backpressure, built by hand (advanced).
// Goal:   make a fast producer wait for a slow consumer.
// Drills: a promise held open until someone else settles it, a waiter
//         queue, FIFO fairness.
// Passes: push resolves immediately while there is room and waits when
//         there is not, pop waits on an empty queue, and both wake in
//         the order they arrived.
//
// ex060 got backpressure for free from a generator, which suspends
// because the language suspends it. This row builds the same guarantee
// out of promises, which is what you need the moment producer and
// consumer are not the same call stack.
//
// The whole technique is one idea: keep the `resolve` function of a
// promise you have not settled yet. `new Promise(resolve => …)` runs its
// executor synchronously, so stashing `resolve` in a list and returning
// the promise gives you a handle someone else can pull later. A waiting
// push is a stashed resolve in one list; a waiting pop is a stashed
// resolve in another.
//
// Order matters and is easy to lose. Waiters must wake FIFO, or a
// producer can starve under load — so the lists are queues, taken from
// the front, and a `Set` or a single pending slot will not do.
//
// The correctness question to hold in your head: when a pop finds a
// waiting push, does the value go through the buffer or straight to the
// consumer? Either works; doing BOTH, or neither, is the bug — one
// duplicates the item and the other drops it.
//
// A warning, as in ex087: getting this wrong HANGS rather than fails.
// A push whose resolve is never called leaves its caller awaiting
// forever, and the test reports nothing at all. Every fact below is
// written so that a correct implementation settles, but a wrong one may
// simply stop.

export class BoundedQueue<T> {
  private readonly buffer: T[] = [];

  constructor(public readonly capacity: number) {}

  /** How many items are buffered right now. */
  get size(): number {
    return this.buffer.length;
  }

  /**
   * TODO: add an item. Resolve at once while the buffer is below
   * capacity; otherwise wait until a pop makes room.
   */
  async push(_item: T): Promise<void> {
    throw new Error("TODO: implement push");
  }

  /**
   * TODO: take the oldest item. Resolve at once when one is buffered;
   * otherwise wait until a push supplies one.
   */
  async pop(): Promise<T> {
    throw new Error("TODO: implement pop");
  }

  /** TODO: how many pushes are currently blocked. */
  get waitingPushes(): number {
    throw new Error("TODO: implement waitingPushes");
  }

  /** TODO: how many pops are currently blocked. */
  get waitingPops(): number {
    throw new Error("TODO: implement waitingPops");
  }
}

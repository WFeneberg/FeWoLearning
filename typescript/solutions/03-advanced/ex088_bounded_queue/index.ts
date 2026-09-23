// Reference solution — exercise 088.
export class BoundedQueue<T> {
  private readonly buffer: T[] = [];

  // Stashed resolve functions. Taken from the front, so waiters wake in
  // the order they arrived.
  private readonly pushWaiters: (() => void)[] = [];
  private readonly popWaiters: ((item: T) => void)[] = [];

  constructor(public readonly capacity: number) {}

  get size(): number {
    return this.buffer.length;
  }

  async push(item: T): Promise<void> {
    // A consumer already waiting takes the item directly: putting it in
    // the buffer as well would duplicate it.
    const waiting = this.popWaiters.shift();
    if (waiting !== undefined) {
      waiting(item);
      return;
    }

    if (this.buffer.length < this.capacity) {
      this.buffer.push(item);
      return;
    }

    // No room. Hold the caller until a pop makes some, then place the
    // item — after waking, not before.
    await new Promise<void>((resolve) => {
      this.pushWaiters.push(resolve);
    });
    this.buffer.push(item);
  }

  async pop(): Promise<T> {
    if (this.buffer.length > 0) {
      const item = this.buffer.shift() as T;
      // Room has appeared: let the oldest blocked producer through.
      this.pushWaiters.shift()?.();
      return item;
    }

    return new Promise<T>((resolve) => {
      this.popWaiters.push(resolve);
    });
  }

  get waitingPushes(): number {
    return this.pushWaiters.length;
  }

  get waitingPops(): number {
    return this.popWaiters.length;
  }
}

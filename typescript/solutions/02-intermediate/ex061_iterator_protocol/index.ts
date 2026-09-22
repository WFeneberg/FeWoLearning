// Reference solution — exercise 061.
export class ManualRange implements Iterable<number> {
  constructor(
    public readonly from: number,
    public readonly to: number,
  ) {}

  [Symbol.iterator](): Iterator<number> {
    // `current` lives in this closure, so each call gets its own cursor
    // and the range can be walked again from the start.
    let current = this.from;
    const end = this.to;
    return {
      next(): IteratorResult<number> {
        if (current >= end) {
          return { value: undefined, done: true };
        }
        const value = current;
        current += 1;
        return { value, done: false };
      },
    };
  }
}

export function closable(
  values: readonly number[],
  onClose: () => void,
): IterableIterator<number> {
  let index = 0;
  let closed = false;

  const iterator: IterableIterator<number> = {
    next(): IteratorResult<number> {
      if (index >= values.length) {
        return { value: undefined, done: true };
      }
      const value = values[index] as number;
      index += 1;
      return { value, done: false };
    },
    // Called by for…of on break/return/throw, and never on a full drain.
    return(): IteratorResult<number> {
      if (!closed) {
        closed = true;
        onClose();
      }
      return { value: undefined, done: true };
    },
    // Returning itself is what makes this single-use.
    [Symbol.iterator]() {
      return iterator;
    },
  };

  return iterator;
}

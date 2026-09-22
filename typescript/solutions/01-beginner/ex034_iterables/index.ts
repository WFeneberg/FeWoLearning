// Reference solution — exercise 034.
export class Range {
  constructor(
    public readonly from: number,
    public readonly to: number,
  ) {}

  // A generator method: the returned object is both an iterator and
  // iterable, so there is nothing else to write.
  *[Symbol.iterator](): Iterator<number> {
    for (let value = this.from; value < this.to; value += 1) {
      yield value;
    }
  }
}

export function sumOf(values: Iterable<number>): number {
  let total = 0;
  for (const value of values) {
    total += value;
  }
  return total;
}

export function take<T>(values: Iterable<T>, count: number): T[] {
  const result: T[] = [];
  if (count <= 0) {
    return result;
  }
  for (const value of values) {
    result.push(value);
    if (result.length === count) {
      // Leaving the loop early is the point: an infinite iterable must not
      // be drained.
      break;
    }
  }
  return result;
}

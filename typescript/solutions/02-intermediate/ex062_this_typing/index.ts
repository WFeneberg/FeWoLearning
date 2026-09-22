// Reference solution — exercise 062.
export interface Counter {
  count: number;
}

// The `this` parameter is erased: at runtime this function takes one
// argument, and `increment.length` is 1.
export function increment(this: Counter, by: number): number {
  this.count += by;
  return this.count;
}

export function ticker(counter: Counter): () => number {
  // A closure over `counter`, not over `this`. Nothing can detach it.
  return () => {
    counter.count += 1;
    return counter.count;
  };
}

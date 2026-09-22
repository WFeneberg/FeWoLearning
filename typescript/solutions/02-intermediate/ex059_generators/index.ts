// Reference solution — exercise 059.
export function* countTo(n: number): Generator<number, number, void> {
  let total = 0;
  for (let value = 1; value <= n; value += 1) {
    total += value;
    yield value;
  }
  // The return value lands on the step where done becomes true.
  return total;
}

export function* runningTotal(start: number): Generator<number, void, number> {
  let total = start;
  while (true) {
    // yield is an EXPRESSION: it produces the value the consumer sent in
    // with the NEXT call to next().
    const received = yield total;
    total += received;
  }
}

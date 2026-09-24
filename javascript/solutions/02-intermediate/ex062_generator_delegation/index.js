// Reference solution — exercise 062.

export function* flatten(value) {
  if (Array.isArray(value)) {
    for (const item of value) yield* flatten(item);
  } else {
    yield value;
  }
}

export function* yieldAndSum(values) {
  let total = 0;
  for (const value of values) {
    yield value;
    total += value;
  }
  return total;
}

export function* countThenTotal(values) {
  // yield* evaluates to the delegate's RETURN value, not to anything it
  // yielded.
  const sum = yield* yieldAndSum(values);
  yield `total: ${sum}`;
}

export function* echo() {
  const received = [];
  let incoming = yield "ready";
  while (incoming !== "done") {
    received.push(incoming);
    incoming = yield `echo: ${incoming}`;
  }
  return received;
}

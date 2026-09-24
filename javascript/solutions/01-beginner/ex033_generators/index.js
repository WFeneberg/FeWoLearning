// Reference solution — exercise 033.

export function* range(start, end, step = 1) {
  for (let value = start; value < end; value += step) yield value;
}

export function* naturals() {
  for (let value = 0; ; value++) yield value;
}

export function* take(iterable, count) {
  if (count <= 0) return;
  let taken = 0;
  for (const value of iterable) {
    yield value;
    if (++taken >= count) return;
  }
}

export function* countedRange(onEnter) {
  onEnter();
  yield 1;
  yield 2;
}

// Reference solution — exercise 032.

export function makeRange(start, end, step = 1) {
  return {
    [Symbol.iterator]() {
      // The cursor lives in the ITERATOR, not in the iterable, which is
      // what makes the object re-iterable.
      let current = start;
      return {
        next() {
          if (current >= end) return { value: undefined, done: true };
          const value = current;
          current += step;
          return { value, done: false };
        },
      };
    },
  };
}

export function isIterable(value) {
  return typeof value?.[Symbol.iterator] === "function";
}

export function pullValues(iterable, count) {
  const iterator = iterable[Symbol.iterator]();
  const values = [];
  while (values.length < count) {
    const { value, done } = iterator.next();
    if (done) break;
    values.push(value);
  }
  return values;
}

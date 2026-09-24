// Reference solution — exercise 041.

export async function* fromPromises(promises) {
  for (const promise of promises) yield await promise;
}

export async function collect(asyncIterable) {
  const values = [];
  for await (const value of asyncIterable) values.push(value);
  return values;
}

export async function* asyncTake(asyncIterable, count) {
  if (count <= 0) return;
  let taken = 0;
  // Breaking out of for await..of calls the source iterator's return(),
  // which is what runs a producer's finally block.
  for await (const value of asyncIterable) {
    yield value;
    if (++taken >= count) break;
  }
}

export function makeAsyncRange(limit) {
  return {
    [Symbol.asyncIterator]() {
      let current = 0;
      return {
        async next() {
          await Promise.resolve();
          if (current >= limit) return { value: undefined, done: true };
          return { value: current++, done: false };
        },
      };
    },
  };
}

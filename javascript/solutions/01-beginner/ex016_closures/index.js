// Reference solution — exercise 016.

export function makeCounter(start = 0) {
  let count = start;
  return {
    next() {
      return count++;
    },
    reset() {
      count = start;
    },
  };
}

export function once(fn) {
  let called = false;
  let result;
  return (...args) => {
    if (!called) {
      called = true; // a separate flag, so a first result of undefined still counts
      result = fn(...args);
    }
    return result;
  };
}

export function indexReaders(count) {
  const readers = [];
  for (let i = 0; i < count; i++) readers.push(() => i);
  return readers;
}

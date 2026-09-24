// Reference solution — exercise 054.

export function memoize(fn) {
  const cache = new Map();
  const memoized = (argument) => {
    // has(), not get() — a stored undefined is a hit, not a miss.
    if (!cache.has(argument)) cache.set(argument, fn(argument));
    return cache.get(argument);
  };
  memoized.clear = () => cache.clear();
  return memoized;
}

export function memoizeBy(fn, keyFn) {
  const cache = new Map();
  return (...args) => {
    const key = keyFn(...args);
    if (!cache.has(key)) cache.set(key, fn(...args));
    return cache.get(key);
  };
}

export function memoizeWeak(fn) {
  const cache = new WeakMap();
  return (object) => {
    if (!cache.has(object)) cache.set(object, fn(object));
    return cache.get(object);
  };
}

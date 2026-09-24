// Reference solution — exercise 023.

export function mapValues(object, fn) {
  return Object.fromEntries(Object.entries(object).map(([key, value]) => [key, fn(value)]));
}

export function invert(object) {
  return Object.fromEntries(Object.entries(object).map(([key, value]) => [value, key]));
}

export function pick(object, keys) {
  return Object.fromEntries(
    keys.filter((key) => Object.hasOwn(object, key)).map((key) => [key, object[key]]),
  );
}

export function assignInto(target, ...sources) {
  return Object.assign(target, ...sources);
}

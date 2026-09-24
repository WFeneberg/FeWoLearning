// Reference solution — exercise 031.

export function valuesViaForOf(iterable) {
  const values = [];
  for (const value of iterable) values.push(value);
  return values;
}

export function keysViaForIn(object) {
  const keys = [];
  // No Object.hasOwn filter here on purpose: the inherited keys are the
  // subject of the exercise.
  for (const key in object) keys.push(key);
  return keys;
}

export function indexKeysOfArray(list) {
  const keys = [];
  for (const key in list) keys.push(key);
  return keys;
}

export function indexedPairs(list) {
  const pairs = [];
  for (const pair of list.entries()) pairs.push(pair);
  return pairs;
}

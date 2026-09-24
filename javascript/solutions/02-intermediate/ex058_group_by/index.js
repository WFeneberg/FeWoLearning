// Reference solution — exercise 058.

export function groupByKey(items, keyFn) {
  return Object.groupBy(items, (item) => keyFn(item));
}

export function groupToMap(items, keyFn) {
  return Map.groupBy(items, (item) => keyFn(item));
}

export function groupWithReduce(items, keyFn) {
  return items.reduce((groups, item) => {
    const key = keyFn(item);
    const bucket = groups.get(key);
    if (bucket === undefined) groups.set(key, [item]);
    else bucket.push(item);
    return groups;
  }, new Map());
}

export function countByKey(items, keyFn) {
  const counts = new Map();
  for (const item of items) {
    const key = keyFn(item);
    counts.set(key, (counts.get(key) ?? 0) + 1);
  }
  return counts;
}

// Reference solution — exercise 059.

export function flattenOnce(list) {
  return list.flat();
}

export function flattenDeep(list) {
  return list.flat(Infinity);
}

export function expand(list, fn) {
  return list.flatMap(fn);
}

export function filterMap(list, predicate, fn) {
  return list.flatMap((item) => (predicate(item) ? [fn(item)] : []));
}

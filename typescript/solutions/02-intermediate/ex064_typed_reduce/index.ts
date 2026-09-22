// Reference solution — exercise 064.
// A is inferred from `seed` alone; `step` is then checked against it.
export function fold<T, A>(
  items: readonly T[],
  seed: A,
  step: (accumulator: A, item: T) => A,
): A {
  let accumulator = seed;
  for (const item of items) {
    accumulator = step(accumulator, item);
  }
  return accumulator;
}

export function groupBy<T, K extends PropertyKey>(
  items: readonly T[],
  selectKey: (item: T) => K,
): Record<K, T[]> {
  const result = {} as Record<K, T[]>;
  for (const item of items) {
    const key = selectKey(item);
    (result[key] ??= []).push(item);
  }
  return result;
}

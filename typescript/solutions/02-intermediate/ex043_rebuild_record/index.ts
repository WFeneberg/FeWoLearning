// Reference solution — exercise 043.
export type MyRecord<K extends PropertyKey, V> = { [P in K]: V };

export function fromKeys<K extends string>(
  keys: readonly K[],
  value: number,
): MyRecord<K, number> {
  const result: Record<string, number> = {};
  for (const key of keys) {
    result[key] = value;
  }
  return result as MyRecord<K, number>;
}

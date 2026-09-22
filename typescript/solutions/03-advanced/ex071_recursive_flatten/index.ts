// Reference solution — exercise 071.
// The recursive call is on E, which is strictly smaller each step, so
// this always terminates.
export type Flatten<T> = T extends readonly (infer E)[] ? Flatten<E> : T;

export type FlattenOnce<T> = T extends readonly (infer E)[] ? E : T;

export function flattenDeep(value: readonly unknown[]): unknown[] {
  const result: unknown[] = [];
  for (const item of value) {
    if (Array.isArray(item)) {
      result.push(...flattenDeep(item as readonly unknown[]));
    } else {
      result.push(item);
    }
  }
  return result;
}

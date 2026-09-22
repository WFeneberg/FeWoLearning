// Reference solution — exercise 060.
export async function* inOrder<T>(
  promises: readonly Promise<T>[],
): AsyncGenerator<T, void, void> {
  for (const promise of promises) {
    // Awaited in argument order, so a promise that settles early still
    // waits for the ones ahead of it.
    yield await promise;
  }
}

export async function collect<T>(source: AsyncIterable<T>): Promise<T[]> {
  const items: T[] = [];
  for await (const item of source) {
    items.push(item);
  }
  return items;
}

export async function* filterAsync<T>(
  source: AsyncIterable<T>,
  keep: (item: T) => boolean,
): AsyncGenerator<T, void, void> {
  for await (const item of source) {
    if (keep(item)) {
      yield item;
    }
  }
}

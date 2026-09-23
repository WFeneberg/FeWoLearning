// Reference solution — exercise 086.
export async function* mapAsync<T, U>(
  source: AsyncIterable<T>,
  transform: (item: T) => U,
): AsyncGenerator<U, void, void> {
  for await (const item of source) {
    yield transform(item);
  }
}

export async function* takeAsync<T>(
  source: AsyncIterable<T>,
  count: number,
): AsyncGenerator<T, void, void> {
  if (count <= 0) {
    return;
  }
  let taken = 0;
  for await (const item of source) {
    yield item;
    taken += 1;
    if (taken === count) {
      // Leaving the loop closes `source`, which resumes the generator
      // above it at its yield and runs that generator's finally.
      break;
    }
  }
}

export async function collectAsync<T>(source: AsyncIterable<T>): Promise<T[]> {
  const items: T[] = [];
  for await (const item of source) {
    items.push(item);
  }
  return items;
}

export async function* naturalsWithCleanup(
  onClose: () => void,
): AsyncGenerator<number, void, void> {
  let value = 0;
  try {
    while (true) {
      yield value;
      value += 1;
    }
  } finally {
    // Runs when the consumer stops early. Code after the loop would not.
    onClose();
  }
}

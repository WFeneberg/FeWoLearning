// Reference solution — exercise 070.
export type MyAwaited<T> = T extends null | undefined
  ? T
  : T extends { then(onfulfilled: (value: infer V) => unknown): unknown }
    ? // Recurse: what `then` hands over may itself be thenable.
      MyAwaited<V>
    : T;

export async function settle<T>(value: T) {
  return await value;
}

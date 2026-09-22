// Reference solution — exercise 065.
export type DeepReadonly<T> = T extends (...args: never[]) => unknown
  ? // Functions first: mapping over one produces {} and loses the signature.
    T
  : T extends readonly (infer E)[]
    ? readonly DeepReadonly<E>[]
    : T extends object
      ? { readonly [K in keyof T]: DeepReadonly<T[K]> }
      : T;

export function deepFreeze<T>(value: T): T {
  if (value === null || (typeof value !== "object" && typeof value !== "function")) {
    return value;
  }
  // Freeze the children before the parent: a frozen parent still has
  // mutable children otherwise, which is the whole point.
  for (const child of Object.values(value as Record<string, unknown>)) {
    deepFreeze(child);
  }
  return Object.freeze(value);
}

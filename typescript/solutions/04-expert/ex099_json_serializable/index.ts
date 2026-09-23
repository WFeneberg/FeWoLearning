// Reference solution — exercise 099.
export type Json<T> = T extends { toJSON(): infer R }
  ? // Date and anything else with a toJSON, BEFORE the object arm — a
    // Date is an object too, and the object arm would win otherwise.
    R
  : T extends (...args: never[]) => unknown
    ? never
    : T extends undefined
      ? never
      : T extends string | number | boolean | null
        ? T
        : T extends readonly (infer E)[]
          ? Json<E>[]
          : T extends Map<unknown, unknown> | Set<unknown>
            ? Record<string, never>
            : T extends object
              ? {
                  // The `as` clause drops the keys the serializer drops.
                  [K in keyof T as Json<T[K]> extends never ? never : K]: Json<T[K]>;
                }
              : never;

export function roundTrip<T>(value: T): Json<T> {
  return JSON.parse(JSON.stringify(value)) as Json<T>;
}

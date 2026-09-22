// Reference solution — exercise 075.
export interface Order {
  id: string;
  customer: { name: string; email: string; address: { city: string; zip: number } };
  total: number;
}

export type Paths<T> = T extends readonly unknown[]
  ? never
  : T extends object
    ? {
        [K in keyof T & string]: T[K] extends readonly unknown[]
          ? K
          : T[K] extends object
            ? K | `${K}.${Paths<T[K]>}`
            : K;
      }[keyof T & string]
    : never;

// `infer` on a template literal is greedy from the LEFT, so Head is one
// segment and Rest is the remainder.
export type ValueAt<T, P extends string> = P extends `${infer Head}.${infer Rest}`
  ? Head extends keyof T
    ? ValueAt<T[Head], Rest>
    : never
  : P extends keyof T
    ? T[P]
    : never;

export function getByPath<P extends Paths<Order>>(
  source: Order,
  path: P,
): ValueAt<Order, P> {
  let current: unknown = source;
  for (const segment of path.split(".")) {
    current = (current as Record<string, unknown>)[segment];
  }
  return current as ValueAt<Order, P>;
}

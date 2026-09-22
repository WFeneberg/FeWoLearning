// Reference solution — exercise 040.
export type ElementOf<T> = T extends readonly (infer E)[] ? E : never;

// One layer: Promise<Promise<string>> gives back Promise<string>.
export type Resolved<T> = T extends Promise<infer R> ? R : T;

// Infer the whole parameter TUPLE, then destructure it. Matching
// `(first: infer P, ...rest: never[]) => unknown` directly looks simpler and
// is wrong: a zero-parameter function is assignable to a signature that
// takes parameters, so `() => void` matches it and P infers as unknown
// rather than never.
export type FirstParam<T> = T extends (...args: infer A) => unknown
  ? A extends readonly [infer P, ...unknown[]]
    ? P
    : never
  : never;

export type Swapped<T> = T extends readonly [infer A, infer B] ? [B, A] : never;

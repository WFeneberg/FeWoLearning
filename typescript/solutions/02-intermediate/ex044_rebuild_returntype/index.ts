// Reference solution — exercise 044.
export type MyReturnType<T> = T extends (...args: never[]) => infer R ? R : never;

// A tuple, so each position keeps its own type.
export type MyParameters<T> = T extends (...args: infer P) => unknown ? P : never;

export type AsyncReturnType<T> = T extends (...args: never[]) => infer R
  ? R extends Promise<infer Resolved>
    ? Resolved
    : R
  : never;

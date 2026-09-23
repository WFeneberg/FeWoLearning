// Reference solution — exercise 090.
export function pipe<A, B>(ab: (a: A) => B): (a: A) => B;
export function pipe<A, B, C>(ab: (a: A) => B, bc: (b: B) => C): (a: A) => C;
export function pipe<A, B, C, D>(
  ab: (a: A) => B,
  bc: (b: B) => C,
  cd: (c: C) => D,
): (a: A) => D;
// Not an overload: callers never see this, which is what lets it be
// loose while every call site above stays checked.
export function pipe(
  ...fns: readonly ((input: never) => unknown)[]
): (input: never) => unknown {
  return (input) =>
    fns.reduce<unknown>((value, fn) => (fn as (v: unknown) => unknown)(value), input);
}

export function compose<A, B>(ab: (a: A) => B): (a: A) => B;
export function compose<A, B, C>(bc: (b: B) => C, ab: (a: A) => B): (a: A) => C;
export function compose<A, B, C, D>(
  cd: (c: C) => D,
  bc: (b: B) => C,
  ab: (a: A) => B,
): (a: A) => D;
export function compose(
  ...fns: readonly ((input: never) => unknown)[]
): (input: never) => unknown {
  // Right to left: the last function runs first.
  return (input) =>
    fns.reduceRight<unknown>(
      (value, fn) => (fn as (v: unknown) => unknown)(value),
      input,
    );
}

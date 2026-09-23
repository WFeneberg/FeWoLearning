// Reference solution — exercise 094.
// Each recursive call IS the whole branch — nothing wraps it — which is
// the shape the compiler unrolls.
export type Reverse<
  T extends readonly unknown[],
  Acc extends readonly unknown[] = [],
> = T extends readonly [infer Head, ...infer Rest] ? Reverse<Rest, [Head, ...Acc]> : Acc;

export type Repeat<
  S extends string,
  N extends number,
  Acc extends string = "",
  Counter extends readonly unknown[] = [],
> = Counter["length"] extends N ? Acc : Repeat<S, N, `${Acc}${S}`, [...Counter, unknown]>;

// Addition via ex093's tuples, threaded through the accumulator.
export type SumOf<
  T extends readonly number[],
  Acc extends readonly unknown[] = [],
> = T extends readonly [infer Head extends number, ...infer Rest extends number[]]
  ? SumOf<Rest, [...Acc, ...BuildTuple<Head>]>
  : Acc["length"];

type BuildTuple<
  N extends number,
  Acc extends readonly unknown[] = [],
> = Acc["length"] extends N ? Acc : BuildTuple<N, [...Acc, unknown]>;

// Reference solution — exercise 093.
export type Tuple<
  N extends number,
  T = unknown,
  Acc extends readonly T[] = [],
> = Acc["length"] extends N ? Acc : Tuple<N, T, [...Acc, T]>;

// Concatenation adds the lengths.
export type Add<A extends number, B extends number> = [
  ...Tuple<A>,
  ...Tuple<B>,
]["length"];

// The match fails when B is the larger, which is where the never comes
// from — the technique has no negatives.
export type Subtract<A extends number, B extends number> = Tuple<A> extends [
  ...Tuple<B>,
  ...infer Rest,
]
  ? Rest["length"]
  : never;

export type AtLeast<A extends number, B extends number> = Tuple<A> extends [
  ...Tuple<B>,
  ...unknown[],
]
  ? true
  : false;

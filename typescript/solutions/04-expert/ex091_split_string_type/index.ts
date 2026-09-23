// Reference solution — exercise 091.
// The recursive call is SPREAD, so the result stays flat; the base case
// keeps the remainder, so "" splits to [""].
export type Split<S extends string, D extends string> = S extends `${infer Head}${D}${infer Rest}`
  ? [Head, ...Split<Rest, D>]
  : [S];

export type Join<
  T extends readonly string[],
  D extends string,
> = T extends readonly [infer Only extends string]
  ? Only
  : T extends readonly [infer Head extends string, ...infer Rest extends string[]]
    ? `${Head}${D}${Join<Rest, D>}`
    : "";

export function split<S extends string, D extends string>(
  text: S,
  delimiter: D,
): Split<S, D> {
  return text.split(delimiter) as Split<S, D>;
}

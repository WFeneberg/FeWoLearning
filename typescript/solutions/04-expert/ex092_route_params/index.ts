// Reference solution — exercise 092.
export type RouteParams<S extends string> = S extends `${string}:${infer Param}/${infer Rest}`
  ? { [K in Param]: string } & RouteParams<`/${Rest}`>
  : S extends `${string}:${infer Param}`
    ? { [K in Param]: string }
    : // eslint-disable-next-line @typescript-eslint/ban-types
      {};

export function matchRoute<S extends string>(
  pattern: S,
  path: string,
): RouteParams<S> | undefined {
  const patternParts = pattern.split("/");
  const pathParts = path.split("/");
  if (patternParts.length !== pathParts.length) {
    return undefined;
  }

  const params: Record<string, string> = {};
  for (let index = 0; index < patternParts.length; index += 1) {
    const expected = patternParts[index] as string;
    const actual = pathParts[index] as string;
    if (expected.startsWith(":")) {
      if (actual.length === 0) {
        return undefined;
      }
      params[expected.slice(1)] = actual;
    } else if (expected !== actual) {
      return undefined;
    }
  }
  return params as RouteParams<S>;
}

// Reference solution — exercise 050.
export type Outcome<T, E = Error> =
  | { ok: true; value: T }
  | { ok: false; error: E };

export type Page<T = unknown> = { items: T[]; total: number };

export function makeList<T = string>(...items: T[]): T[] {
  // With no arguments there is nothing to infer from, so the default
  // applies and the result is string[].
  return [...items];
}

// Exercise 096 — a query builder that knows its own row shape (expert).
// Goal:   let a chain of calls decide the type of what comes out.
// Drills: accumulating a key union through a fluent chain, Pick over
//         that union, a value checked against the column it names.
// Passes: the result type is exactly the selected columns, and a wrong
//         column or a wrong value type is a compile error.
//
// ex078's builder accumulated an object type; this accumulates a UNION
// OF KEYS, which is the smaller and more common case. `select` returns
// `Query<T, Selected | K>`, so the chain remembers everything chosen so
// far, and `run` hands back `Pick<T, Selected>[]` — the row shape the
// caller actually asked for, not the whole table.
//
// `where` is the other half and reuses ex049: `<K extends keyof T>(key:
// K, value: T[K])` ties the value to the column named in the same call,
// so comparing a numeric id against a string is an error at that
// argument rather than an empty result set at runtime.
//
// Two things worth noticing about the design. Selecting nothing gives
// `Pick<T, never>`, which is `{}` — honest, and arguably what an empty
// SELECT deserves. And `select` is additive rather than replacing, so
// two calls union rather than the second winning; that is a choice, and
// the facts pin it down so a later reader does not have to guess.
//
// The stub's methods take a plain `keyof T` and `unknown`, so the tests
// can chain against it. Introducing the type parameters is the work.

export interface Row {
  id: number;
  name: string;
  email: string;
  active: boolean;
}

export interface Query<T, Selected extends keyof T> {
  /** Phantom, so Selected is comparable. Never assigned. */
  readonly selected?: Selected;

  /** TODO: add these columns to the selection. */
  select(...keys: (keyof T)[]): Query<T, keyof T>;

  /** TODO: keep only rows whose `key` equals `value`. The value must
   *  match that column's own type. */
  where(key: keyof T, value: unknown): Query<T, Selected>;

  /** TODO: apply the filters and project onto the selection. */
  run(rows: readonly T[]): Pick<T, Selected>[];
}

/** TODO: an empty query over T, with nothing selected yet. */
export function from<T>(): Query<T, never> {
  throw new Error("TODO: implement from");
}

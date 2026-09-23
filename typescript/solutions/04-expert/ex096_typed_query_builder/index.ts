// Reference solution — exercise 096.
export interface Row {
  id: number;
  name: string;
  email: string;
  active: boolean;
}

export interface Query<T, Selected extends keyof T> {
  readonly selected?: Selected;

  // Additive: the union grows, so two selects combine.
  select<K extends keyof T>(...keys: K[]): Query<T, Selected | K>;

  // T[K] in a parameter position ties the value to the column named in
  // the same call (ex049).
  where<K extends keyof T>(key: K, value: T[K]): Query<T, Selected>;

  run(rows: readonly T[]): Pick<T, Selected>[];
}

export function from<T>(): Query<T, never> {
  const keys: (keyof T)[] = [];
  const filters: { key: keyof T; value: T[keyof T] }[] = [];

  const query: Query<T, never> = {
    select(...added) {
      keys.push(...(added as (keyof T)[]));
      return query as never;
    },
    where(key, value) {
      filters.push({ key, value: value as T[keyof T] });
      return query;
    },
    run(rows) {
      return rows
        .filter((row) => filters.every((filter) => row[filter.key] === filter.value))
        .map((row) => {
          const projected: Partial<T> = {};
          for (const key of keys) {
            projected[key] = row[key];
          }
          return projected as Pick<T, never>;
        });
    },
  };

  return query;
}

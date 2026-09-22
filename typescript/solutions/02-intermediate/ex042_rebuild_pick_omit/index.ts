// Reference solution — exercise 042.
export interface Row {
  readonly id: string;
  label?: string;
  count: number;
}

export type MyPick<T, K extends keyof T> = { [P in K]: T[P] };

// Constrained to keyof T, unlike the built-in Omit, so a misspelt key is
// an error here rather than a silent no-op.
export type MyOmit<T, K extends keyof T> = {
  [P in keyof T as P extends K ? never : P]: T[P];
};

export function omitAt<K extends keyof Row>(
  row: Row,
  keys: readonly K[],
): MyOmit<Row, K> {
  const result: Record<string, unknown> = { ...row };
  for (const key of keys) {
    delete result[key as string];
  }
  return result as MyOmit<Row, K>;
}

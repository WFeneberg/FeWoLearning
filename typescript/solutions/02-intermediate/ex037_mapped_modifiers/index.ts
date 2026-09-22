// Reference solution — exercise 037.
export interface Row {
  readonly id: string;
  label?: string;
  count: number;
}

// Homomorphic: the modifiers come across on their own.
export type Same<T> = { [K in keyof T]: T[K] };

export type Solid<T> = { -readonly [K in keyof T]-?: T[K] };

export type Loose<T> = { +readonly [K in keyof T]+?: T[K] };

export function solidify(row: Row): Solid<Row> {
  return {
    id: row.id,
    label: row.label ?? "",
    count: row.count,
  };
}

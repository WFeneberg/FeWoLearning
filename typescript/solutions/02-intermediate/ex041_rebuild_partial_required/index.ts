// Reference solution — exercise 041.
export interface Row {
  readonly id: string;
  label?: string;
  count: number;
}

export type MyPartial<T> = { [P in keyof T]?: T[P] };

export type MyRequired<T> = { [P in keyof T]-?: T[P] };

// Two mapped types intersected: the keys NOT in K keep their modifiers
// (the `as` clause filters them), and the keys in K are re-emitted optional.
export type PartialBy<T, K extends keyof T> = {
  [P in keyof T as P extends K ? never : P]: T[P];
} & {
  [P in K]?: T[P];
};

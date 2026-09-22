// Reference solution — exercise 016.
export function identity<T>(value: T): T {
  return value;
}

export function longer<T extends { length: number }>(a: T, b: T): T {
  return b.length > a.length ? b : a;
}

// Reference solution — exercise 006.
export type Coordinate = [latitude: number, longitude: number];

// The rest element carries the "at least one" promise: position 0 is
// required, everything after it is optional.
export type NonEmptyStrings = [string, ...string[]];

export function formatCoordinate(at: Coordinate): string {
  const [latitude, longitude] = at;
  return `${latitude.toFixed(2)},${longitude.toFixed(2)}`;
}

export function head(items: NonEmptyStrings): string {
  // No `!` and no fallback: the tuple type guarantees index 0 exists.
  return items[0];
}

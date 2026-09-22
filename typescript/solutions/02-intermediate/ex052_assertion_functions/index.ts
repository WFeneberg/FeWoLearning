// Reference solution — exercise 052.
export function assertString(value: unknown): asserts value is string {
  if (typeof value !== "string") {
    throw new TypeError(`expected a string, got ${typeof value}`);
  }
}

export function assertDefined<T>(value: T | null | undefined): asserts value is T {
  if (value === null || value === undefined) {
    throw new TypeError("expected a value");
  }
}

// The narrowed type is ex006's non-empty tuple, which is what removes the
// undefined from items[0] under noUncheckedIndexedAccess.
export function assertNonEmpty<T>(
  items: readonly T[],
): asserts items is readonly [T, ...T[]] {
  if (items.length === 0) {
    throw new RangeError("expected at least one item");
  }
}

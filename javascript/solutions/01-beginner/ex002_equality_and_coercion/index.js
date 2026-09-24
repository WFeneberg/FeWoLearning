// Reference solution — exercise 002.

export function isSameValueZero(a, b) {
  if (a === b) return true;
  // The only pair `===` misses: two NaNs. NaN is the one value not equal
  // to itself, which is also how you detect it without Number.isNaN.
  return a !== a && b !== b;
}

export function dedupe(values) {
  // Set uses SameValueZero, which is the whole point of the row.
  return [...new Set(values)];
}

export function normalizeZero(value) {
  return Object.is(value, -0) ? 0 : value;
}
